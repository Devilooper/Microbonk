using Microbonk.Features.Collectibles.Runtime.Components;
using Microbonk.Features.Collectibles.Runtime.Components.Homing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Microbonk.Features.Collectibles.Runtime.Jobs
{
    /// This Job has two responsibilities as to avoid multiple iterations over the same data
    [BurstCompile]
    [WithAll(typeof(CollectibleTag), typeof(HomingTowardsTarget))]
    public partial struct MoveAndMarkCollectedJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;

        [ReadOnly] public ComponentLookup<LocalToWorld> TargetPositions;
        public float Speed;
        public float CompleteRadius;
        public float DeltaTime;

        private void Execute(
            [ChunkIndexInQuery] int index, Entity collectibleEntity,
            ref LocalTransform collectibleTransform, in HomingTowardsTarget homingTarget)
        {
            float3 targetPos = this.TargetPositions[homingTarget.Target].Position;
            float3 d = targetPos - collectibleTransform.Position;
            float dist = math.length(d);

            // Handle collection
            bool isWithinCompleteRadius = dist <= this.CompleteRadius || dist <= 1e-6f;
            if (isWithinCompleteRadius)
            {
                var evt = this.Ecb.CreateEntity(index);
                this.Ecb.AddComponent(index, evt, new CollectedEvent
                {
                    Collectible = collectibleEntity,
                    Target = homingTarget.Target,
                    Position = collectibleTransform.Position
                });
                this.Ecb.AddComponent<DestroyMe>(index, collectibleEntity);
                return;
            }

            // Handle movement
            float3 dir = d / math.max(dist, 1e-6f);
            float step = this.Speed * this.DeltaTime;
            collectibleTransform.Position += dir * step;
        }
    }
}