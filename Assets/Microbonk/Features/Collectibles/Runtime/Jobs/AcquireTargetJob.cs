using Microbonk.Features.Collectibles.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Microbonk.Features.Collectibles.Runtime.Jobs
{
    [BurstCompile]
    [WithAll(typeof(CollectibleTag))]
    [WithNone(typeof(HomingTowardsTarget))]
    public partial struct AcquireTargetJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;

        [ReadOnly] public NativeArray<Entity> TargetEntities;
        [ReadOnly] public NativeArray<LocalToWorld> TargetTransforms;
        public float AcquireRadiusSq;

        // This could be split into methods, but it's kept concise to ensure that the compiler can easily optimize it
        private void Execute([ChunkIndexInQuery] int index, Entity collectibleEntity,
            in LocalTransform collectibleTransform)
        {
            // Find nearest target
            float3 p = collectibleTransform.Position;
            float bestSq = float.MaxValue;
            int bestIdx = -1;

            for (int i = 0; i < this.TargetTransforms.Length; i++)
            {
                float3 d = this.TargetTransforms[i].Position - p;
                float s = math.lengthsq(d);
                if (s < bestSq)
                {
                    bestSq = s;
                    bestIdx = i;
                }
            }

            // Start homing only if within Acquire radius
            if (bestIdx >= 0 && bestSq <= this.AcquireRadiusSq)
            {
                this.Ecb.AddComponent(index, collectibleEntity,
                    new HomingTowardsTarget { Target = this.TargetEntities[bestIdx] });
            }
        }
    }
}