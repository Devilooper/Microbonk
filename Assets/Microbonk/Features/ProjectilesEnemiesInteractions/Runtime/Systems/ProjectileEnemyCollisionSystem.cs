using Microbonk.Features.Collectibles.Runtime.Components;
using Microbonk.Features.Projectiles.Runtime.Components;
using Microbonk.Features.ProjectilesEnemiesInteractions.Runtime.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Microbonk.Features.ProjectilesEnemiesInteractions.Runtime.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ProjectileEnemyCollisionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollectibleDropSettingsSingleton>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var projectiles = SystemAPI.GetComponentLookup<ProjectileTag>(true);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            
            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            var endSim = state.World.GetOrCreateSystemManaged<EndFixedStepSimulationEntityCommandBufferSystem>();

            var destroyJob = new DestroyBothOnInteractionJob
            {
                Projectiles = projectiles,
                Transforms = transforms,
                Ecb = endSim.CreateCommandBuffer(),
                DropSettings = SystemAPI.GetSingleton<CollectibleDropSettingsSingleton>()
            }.Schedule(simulationSingleton, state.Dependency);
            endSim.AddJobHandleForProducer(destroyJob);
            state.Dependency = destroyJob;
        }
    }
}