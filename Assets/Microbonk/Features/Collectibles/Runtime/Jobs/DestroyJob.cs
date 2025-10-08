using Microbonk.Features.Collectibles.Runtime.Components.Homing;
using Unity.Burst;
using Unity.Entities;

namespace Microbonk.Features.Collectibles.Runtime.Jobs
{
    [BurstCompile]
    [WithAll(typeof(DestroyMe))]
    public partial struct DestroyJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;

        private void Execute([ChunkIndexInQuery] int index, Entity e)
        {
            this.Ecb.DestroyEntity(index, e);
        }
    }
}