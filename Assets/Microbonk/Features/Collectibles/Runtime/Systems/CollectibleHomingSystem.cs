using Microbonk.Features.Collectibles.Runtime.Components;
using Microbonk.Features.Collectibles.Runtime.Components.Homing;
using Microbonk.Features.Collectibles.Runtime.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Microbonk.Features.Collectibles.Runtime.Systems
{
    [BurstCompile]
    public partial struct CollectibleHomingSystem : ISystem
    {
        private EntityQuery homingTargetsQuery;
        [ReadOnly] private ComponentLookup<LocalToWorld> targetsPositions;

        public void OnCreate(ref SystemState state)
        {
            this.homingTargetsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<HomingTargetTag, LocalToWorld>()
                .Build(ref state);

            this.targetsPositions = state.GetComponentLookup<LocalToWorld>(true);

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

            var endSim = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            var ecbSimulate = endSim.CreateCommandBuffer().AsParallelWriter();
            var ecbDestroy = endSim.CreateCommandBuffer().AsParallelWriter();

            var acquireHandle = new AcquireTargetJob
            {
                TargetEntities = targetEntities,
                TargetTransforms = targetTransforms,
                AcquireRadiusSq = acquireRadius * acquireRadius,
                Ecb = ecbSimulate
            }.ScheduleParallel(state.Dependency);
            endSim.AddJobHandleForProducer(acquireHandle);

            var moveHandle = new MoveAndMarkCollectedJob
            {
                TargetPositions = this.targetsPositions,
                Speed = speed,
                CompleteRadius = completeRadius,
                DeltaTime = SystemAPI.Time.DeltaTime,
                Ecb = ecbSimulate
            }.ScheduleParallel(acquireHandle);
            endSim.AddJobHandleForProducer(moveHandle);

            var destroyHandle = new DestroyJob
            {
                Ecb = ecbDestroy
            }.ScheduleParallel(moveHandle);
            endSim.AddJobHandleForProducer(destroyHandle);

            state.Dependency = destroyHandle;
        }
    }
}