using Unity.Entities;

namespace Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning
{
    public struct ProjectileSpawner : IComponentData
    {
        public Entity ToSpawn;
        public float SpawnIntervalSeconds;
    }
}