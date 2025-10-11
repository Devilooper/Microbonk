using Unity.Entities;

namespace Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning
{
    public struct SpawnerCooldown : IComponentData
    {
        public float SecondsRemaining;
    }
}