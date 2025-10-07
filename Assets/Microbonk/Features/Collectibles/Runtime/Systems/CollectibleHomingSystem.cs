using Game.Features.Collectibles.Runtime.Components.Homing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Features.Collectibles.Runtime.Systems
{
    /// <summary>
    ///     Moves collectible towards player if in range
    /// </summary>
    [BurstCompile]
    [WithAll(typeof(CollectibleTag))]
    public partial struct CollectibleHomingSystem : ISystem
    {
        private EntityQuery homingTargetQuery;

        public void OnCreate(ref SystemState state)
        {
            this.homingTargetQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<HomingTargetTag, LocalTransform>()
                .Build(ref state);
            

            state.RequireForUpdate<CollectibleHomingSettingsSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var (speed, acquireRadius, completeRadius) = SystemAPI.GetSingleton<CollectibleHomingSettingsSingleton>();

            var foundHomingTargets =
                this.homingTargetQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

            new HomingJob
                {
                    HomingTargets = foundHomingTargets,
                    Radius = acquireRadius,
                    Speed = speed,
                    DeltaTime = SystemAPI.Time.DeltaTime
                }
                .ScheduleParallel();
            foundHomingTargets.Dispose();
        }


        [BurstCompile]
        [WithAll(typeof(CollectibleTag))]
        public partial struct HomingJob : IJobEntity
        {
            [ReadOnly] public NativeArray<LocalTransform> HomingTargets;
            public float Radius;
            public float Speed;
            public float DeltaTime;

            private void Execute(ref LocalTransform collectibleTransform)
            {
                foreach (LocalTransform homingTarget in this.HomingTargets)
                {
                    float3 toTarget = homingTarget.Position - collectibleTransform.Position;
                    float distance = math.length(toTarget);
                    if (distance <= this.Radius)
                    {
                        collectibleTransform.Position += toTarget * this.Speed * this.DeltaTime;
                    }
                }
            }
        }
    }
}