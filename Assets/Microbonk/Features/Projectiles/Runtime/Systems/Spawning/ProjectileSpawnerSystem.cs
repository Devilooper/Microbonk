using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Unity.Burst;
using Unity.Entities;

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
            var ecbSingleton = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            new SpawnProjectilesJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Ecb = ecb
            }.ScheduleParallel();
        }
    }
}