using Microbonk.Features.Collectibles.Runtime.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace Microbonk.Features.Collectibles.Runtime.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(CollectibleRespondersGroup))]
    public partial struct DestroyMarkedCollectiblesSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var endSim = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            var ecbDestroy = endSim.CreateCommandBuffer().AsParallelWriter();

            var destroyHandle = new DestroyJob
            {
                Ecb = ecbDestroy
            }.ScheduleParallel(state.Dependency);
            endSim.AddJobHandleForProducer(destroyHandle);

            state.Dependency = destroyHandle;
        }
    }
}