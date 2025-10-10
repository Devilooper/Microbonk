using Microbonk.Features.Projectiles.Runtime.Components;
using Microbonk.Features.Projectiles.Runtime.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace Microbonk.Features.Projectiles.Runtime.Systems
{
    [BurstCompile]
    public partial struct ProjectileMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileSpeed>();
        }

        public void OnUpdate(ref SystemState state)
        {
            new ProjectileMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel();
        }
    }
}