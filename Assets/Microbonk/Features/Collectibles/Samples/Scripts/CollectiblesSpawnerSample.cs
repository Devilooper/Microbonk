using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Microbonk.Features.Collectibles.Samples.Scripts
{
    /// <summary>
    ///     Editor-exposed configuration of the collectibles spawner
    /// </summary>
    public class CollectiblesSpawnerSampleAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public int Amount;
        public int Radius;

        public class CollectiblesSpawnerSampleBaker : Baker<CollectiblesSpawnerSampleAuthoring>
        {
            public override void Bake(CollectiblesSpawnerSampleAuthoring authoring)
            {
                Entity entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CollectiblesSpawnerSampleData
                {
                    ToSpawn = entityPrefab,
                    Amount = authoring.Amount,
                    Radius = authoring.Radius
                });
            }
        }
    }

    /// <summary>
    ///     Configuration data singleton
    /// </summary>
    public struct CollectiblesSpawnerSampleData : IComponentData
    {
        public Entity ToSpawn;
        public int Amount;
        public float Radius;
    }

    /// <summary>
    ///     Spawns collectibles on start. No need for Job, it's a deferred instantiation
    /// </summary>
    public partial struct CollectiblesSpawnerSampleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollectiblesSpawnerSampleData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (configuration, entity) in
                     SystemAPI.Query<RefRO<CollectiblesSpawnerSampleData>>()
                         .WithEntityAccess())
            {
                var random = new Random(1);
                for (int i = 0; i < configuration.ValueRO.Amount; i++)
                {
                    Entity instance = ecb.Instantiate(configuration.ValueRO.ToSpawn);
                    float angle = random.NextFloat(2 * math.PI);
                    var position = new float3(math.sin(angle), 0, math.cos(angle));
                    position *= random.NextFloat(configuration.ValueRO.Radius);
                    ecb.SetComponent(instance, LocalTransform.FromPosition(position));
                }

                ecb.RemoveComponent<CollectiblesSpawnerSampleData>(entity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}