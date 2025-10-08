using Microbonk.Features.Collectibles.Runtime.Components;
using Microbonk.Features.Collectibles.Runtime.Components.Homing;
using Microbonk.Features.Collectibles.Runtime.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Microbonk.Features.Collectibles.Runtime.Systems
{
    /// <summary>
    ///     Moves collectible towards player if in range
    /// </summary>
    [BurstCompile]
    public partial struct CollectibleHomingSystem : ISystem
    {
        private EntityQuery homingTargetsQuery;

        [ReadOnly] private ComponentLookup<LocalToWorld> targetsPositions;
        // ^^^^^ isn't LocalTransform too broad? can I / should I restrict it to targets specifically?

        public void OnCreate(ref SystemState state)
        {
            this.homingTargetsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<HomingTargetTag, LocalToWorld>()
                .Build(ref state);

            this.targetsPositions = state.GetComponentLookup<LocalToWorld>(isReadOnly: true);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CollectibleHomingSettingsSingleton>();
            state.RequireForUpdate<CollectibleTag>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate(this.homingTargetsQuery);
        }

        public void OnUpdate(ref SystemState state)
        {
            var (speed, acquireRadius, completeRadius) = SystemAPI.GetSingleton<CollectibleHomingSettingsSingleton>();

            var targetEntities = this.homingTargetsQuery.ToEntityArray(state.WorldUpdateAllocator);
            var targetTransforms =
                this.homingTargetsQuery.ToComponentDataArray<LocalToWorld>(state.WorldUpdateAllocator);

            this.targetsPositions.Update(ref state);

            var ecnSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var ecb = ecnSystem.CreateCommandBuffer().AsParallelWriter();

            var acquireHandle = new AcquireTargetJob
            {
                TargetEntities = targetEntities,
                TargetTransforms = targetTransforms,
                AcquireRadiusSq = acquireRadius * acquireRadius,
                Ecb = ecb
            }.ScheduleParallel(state.Dependency);
            ecnSystem.AddJobHandleForProducer(acquireHandle);
            
            var moveHandle = new MoveAndMarkCollectedJob
            {
                TargetLT = this.targetsPositions,
                Speed = speed,
                CompleteRadius = completeRadius,
                DeltaTime = SystemAPI.Time.DeltaTime,
                Ecb = ecb
            }.ScheduleParallel(acquireHandle);
            ecnSystem.AddJobHandleForProducer(moveHandle);

            // var destroyHandle = new DestroyJob
            // {
            //     Ecb = ecb
            // }.ScheduleParallel(moveHandle);
            // ecnSystem.AddJobHandleForProducer(destroyHandle);
        }
    }
}