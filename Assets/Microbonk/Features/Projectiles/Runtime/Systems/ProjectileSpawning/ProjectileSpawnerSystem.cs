using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Microbonk.Features.Projectiles.Runtime.Systems.ProjectileSpawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileSpawnerCircularMovementSystem))]
    public partial struct ProjectileSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CircularMovementAroundTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton =
                SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (spawner, cooldown, spawnerTransform) in SystemAPI
                         .Query<
                             RefRO<ProjectileSpawner>,
                             RefRW<SpawnerCooldown>,
                             RefRW<LocalTransform>>())
            {
                ref var remainingCooldown = ref cooldown.ValueRW;
                remainingCooldown.SecondsRemaining -= SystemAPI.Time.DeltaTime;

                if (remainingCooldown.SecondsRemaining > 0f)
                {
                    continue;
                }
                
                var entity = ecb.Instantiate(spawner.ValueRO.ToSpawn);
                ecb.SetComponent(entity, spawnerTransform.ValueRO);
                remainingCooldown.SecondsRemaining = spawner.ValueRO.SpawnIntervalSeconds;
            }
        }
    }
}