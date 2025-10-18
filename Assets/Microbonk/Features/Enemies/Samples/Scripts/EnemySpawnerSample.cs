using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Microbonk.Features.Enemies.Samples.Scripts
{
    public class EnemySpawnerSampleAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public int MaxEnemiesCount;
        public float MinRadius;
        public float MaxRadius;

        public class EnemySpawnerSampleBaker : Baker<EnemySpawnerSampleAuthoring>
        {
            public override void Bake(EnemySpawnerSampleAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EnemySpawner
                {
                    ToSpawn = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                    MaxEnemiesCount = authoring.MaxEnemiesCount,
                    MinRadius = authoring.MinRadius,
                    MaxRadius = authoring.MaxRadius,
                });

                AddBuffer<EntitySpawnedBySpawner>(entity);
            }
        }
    }

    public struct EnemySpawner : IComponentData
    {
        public Entity ToSpawn;
        public int MaxEnemiesCount;
        public float MinRadius;
        public float MaxRadius;
    }

    public struct EntitySpawnedBySpawner : IBufferElementData
    {
        public Entity Value;
    }

    [BurstCompile]
    public partial struct EnemySpawnerSampleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<EnemySpawner>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var entityManager = state.EntityManager;

            foreach (var (configuration, entity) in
                     SystemAPI.Query<RefRO<EnemySpawner>>()
                         .WithEntityAccess())
            {
                DynamicBuffer<EntitySpawnedBySpawner> spawned = entityManager.GetBuffer<EntitySpawnedBySpawner>(entity);
                TrimDestroyed(spawned, entityManager);

                int maxEnemies = configuration.ValueRO.MaxEnemiesCount;
                if (spawned.Length >= configuration.ValueRO.MaxEnemiesCount)
                {
                    continue;
                }

                int toSpawn = maxEnemies - spawned.Length;
                SpawnEnemies(configuration.ValueRO, toSpawn, ref ecb, entity);
            }
        }

        private static void TrimDestroyed(DynamicBuffer<EntitySpawnedBySpawner> buffer, EntityManager entityManager)
        {
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                Entity trackedEntity = buffer[i].Value;
                if (!entityManager.Exists(trackedEntity))
                {
                    buffer.RemoveAt(i);
                }
            }
        }

        private static void SpawnEnemies(in EnemySpawner spawner, int count, ref EntityCommandBuffer ecb,
            Entity spawnerEntity)
        {
            var random = new Random(1);
            for (int i = 0; i < count; i++)
            {
                Entity instance = ecb.Instantiate(spawner.ToSpawn);

                float angle = random.NextFloat(2 * math.PI);
                float radius = random.NextFloat(spawner.MinRadius, spawner.MaxRadius);
                var position = new float3(math.sin(angle) * radius, 0f, math.cos(angle) * radius);

                ecb.SetComponent(instance, LocalTransform.FromPosition(position));
                ecb.AppendToBuffer(spawnerEntity, new EntitySpawnedBySpawner { Value = instance });
            }
        }
    }
}