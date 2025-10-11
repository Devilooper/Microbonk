using Unity.Entities;

namespace Microbonk.Features.Projectiles.Runtime.Components
{
    public struct ProjectileLifetime : IComponentData
    {
        public float SecondsRemaining;
    }
}