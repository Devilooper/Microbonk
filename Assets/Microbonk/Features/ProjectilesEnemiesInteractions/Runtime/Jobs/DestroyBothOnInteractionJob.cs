using Microbonk.Features.Collectibles.Runtime.Components;
using Microbonk.Features.Projectiles.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Microbonk.Features.ProjectilesEnemiesInteractions.Runtime.Jobs
{
    [BurstCompile]
    public struct DestroyBothOnInteractionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<ProjectileTag> Projectiles;
        [ReadOnly] public ComponentLookup<LocalTransform> Transforms;
        [ReadOnly] public CollectibleDropSettingsSingleton DropSettings;

        public EntityCommandBuffer Ecb;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (!(this.Projectiles.HasComponent(triggerEvent.EntityA) ||
                  this.Projectiles.HasComponent(triggerEvent.EntityB)))
            {
                return;
            }

            this.Ecb.DestroyEntity(triggerEvent.EntityA);
            this.Ecb.DestroyEntity(triggerEvent.EntityB);
            var spawned = this.Ecb.Instantiate(this.DropSettings.DropPrefab);
            this.Ecb.SetComponent(spawned, LocalTransform.FromPosition(this.Transforms[triggerEvent.EntityA].Position));
        }
    }
}