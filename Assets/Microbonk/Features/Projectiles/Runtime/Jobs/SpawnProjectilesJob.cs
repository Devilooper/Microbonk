using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Microbonk.Features.Projectiles.Runtime.Systems.ProjectileSpawning
{
    [BurstCompile]
    public partial struct SpawnProjectilesJob : IJobEntity
    {
        public float DeltaTime;

        public EntityCommandBuffer.ParallelWriter Ecb;

        private void Execute(
            [ChunkIndexInQuery] int index,
            in ProjectileSpawner spawner,
            ref SpawnerCooldown cooldown,
            in LocalTransform spawnerTransform)
        {
            cooldown.SecondsRemaining -= this.DeltaTime;

            if (cooldown.SecondsRemaining > 0f)
            {
                return;
            }

            var entity = this.Ecb.Instantiate(index, spawner.ToSpawn);
            this.Ecb.SetComponent(index, entity, spawnerTransform);
            cooldown.SecondsRemaining = spawner.SpawnIntervalSeconds;
        }
    }
}