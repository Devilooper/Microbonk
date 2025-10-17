using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Microbonk.Features.Projectiles.Runtime.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ProjectileSpawnerCircularMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CircularMovementAroundTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            new CircularMovementJob
            {
                ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
                OriginPositions = SystemAPI.GetComponentLookup<LocalToWorld>(true)
            }.ScheduleParallel();
        }
    }
}
