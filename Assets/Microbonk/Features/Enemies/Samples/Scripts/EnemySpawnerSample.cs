using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Microbonk.Features.Enemies.Samples.Scripts
{
    /// <summary>
    ///     Editor-exposed configuration of the enemy spawner sample
    /// </summary>
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
                Entity entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                uint randomSeed = math.hash(new float3(authoring.MinRadius, authoring.MaxRadius, authoring.MaxEnemiesCount));
                if (randomSeed == 0u)
                {
                    randomSeed = 1u;
                }

                AddComponent(entity, new EnemySpawnerSampleData
                {
                    ToSpawn = entityPrefab,
                    MaxEnemiesCount = math.max(authoring.MaxEnemiesCount, 0),
                    MinRadius = math.max(0f, authoring.MinRadius),
                    MaxRadius = math.max(authoring.MinRadius, authoring.MaxRadius),
                    RandomState = randomSeed
                });

                AddBuffer<EnemySpawnerSampleTrackedEntity>(entity);
            }
        }
    }

    /// <summary>
    ///     Configuration data singleton for enemy spawning
    /// </summary>
    public struct EnemySpawnerSampleData : IComponentData
    {
        public Entity ToSpawn;
        public int MaxEnemiesCount;
        public float MinRadius;
        public float MaxRadius;
        public uint RandomState;
    }

    /// <summary>
    ///     Tracks the enemies spawned by a specific spawner
    /// </summary>
    public struct EnemySpawnerSampleTrackedEntity : IBufferElementData
    {
        public Entity Value;
    }

    /// <summary>
    ///     Maintains a target enemy count, spawning replacements when needed
    /// </summary>
    public partial struct EnemySpawnerSampleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnerSampleData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entityManager = state.EntityManager;

            foreach (var (configuration, entity) in
                     SystemAPI.Query<RefRW<EnemySpawnerSampleData>>()
                         .WithEntityAccess())
            {
                ref var data = ref configuration.ValueRW;
                if (data.ToSpawn == Entity.Null)
                {
                    continue;
                }
                

                DynamicBuffer<EnemySpawnerSampleTrackedEntity> buffer = entityManager.GetBuffer<EnemySpawnerSampleTrackedEntity>(entity);

                TrimDestroyed(buffer, entityManager);

                int targetCount = math.max(0, data.MaxEnemiesCount);
                if (buffer.Length >= targetCount)
                {
                    continue;
                }

                int toSpawn = targetCount - buffer.Length;
                SpawnEnemies(ref data, toSpawn, ref ecb, entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private static void TrimDestroyed(DynamicBuffer<EnemySpawnerSampleTrackedEntity> buffer, EntityManager entityManager)
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

        private static void SpawnEnemies(ref EnemySpawnerSampleData data, int count, ref EntityCommandBuffer ecb, Entity spawnerEntity)
        {
            var random = new Random(data.RandomState == 0u ? 1u : data.RandomState);
            float minRadius = math.max(0f, data.MinRadius);
            float maxRadius = math.max(minRadius, data.MaxRadius);

            for (int i = 0; i < count; i++)
            {
                Entity instance = ecb.Instantiate(data.ToSpawn);
                float angle = random.NextFloat(2 * math.PI);
                float radius = maxRadius <= minRadius
                    ? minRadius
                    : random.NextFloat(minRadius, maxRadius);
                var position = new float3(math.sin(angle) * radius, 0f, math.cos(angle) * radius);
                ecb.SetComponent(instance, LocalTransform.FromPosition(position));
                ecb.AppendToBuffer(spawnerEntity, new EnemySpawnerSampleTrackedEntity { Value = instance });
            }

            data.RandomState = random.NextUInt();
        }
    }
}
