using Microbonk.Features.Enemies.Runtime.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Microbonk.Features.Enemies.Runtime.Systems
{
    [BurstCompile]
    public partial struct EnemyMovementSystem : ISystem
    {
        private ComponentLookup<LocalTransform> targetTransformLookup;

        public void OnCreate(ref SystemState state)
        {
            this.targetTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            this.targetTransformLookup.Update(ref state);
            new EnemyMovementJob
                {
                    CollisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
                    TargetTransformLookup = this.targetTransformLookup
                }
                .ScheduleParallel();
        }
    }
}