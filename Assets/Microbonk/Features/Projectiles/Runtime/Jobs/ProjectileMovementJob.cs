using Microbonk.Features.Projectiles.Runtime.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Microbonk.Features.Projectiles.Runtime.Jobs
{
    [BurstCompile]
    public partial struct ProjectileMovementJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(in ProjectileSpeed speed, ref LocalTransform transform)
        {
            transform.Position += transform.Forward() * speed.Speed * this.DeltaTime;
        }
    }
}