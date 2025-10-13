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
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            new EnemyMovementJob
                {
                    PhysicsWorld = physicsWorld,
                    TargetTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true)
                }
                .ScheduleParallel();
        }
    }
}