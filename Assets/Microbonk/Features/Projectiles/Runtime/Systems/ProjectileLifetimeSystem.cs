using Microbonk.Features.Projectiles.Runtime.Components;
using Unity.Burst;
using Unity.Entities;

namespace Microbonk.Features.Projectiles.Runtime.Systems
{
    [BurstCompile]
    public partial struct ProjectileLifetimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProjectileLifetime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((var lifetimeRef, Entity entity) in SystemAPI
                         .Query<RefRW<ProjectileLifetime>>()
                         .WithEntityAccess())
            {
                ref var lifetime = ref lifetimeRef.ValueRW;
                lifetime.SecondsRemaining -= SystemAPI.Time.DeltaTime;
                if (lifetime.SecondsRemaining <= 0f)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}