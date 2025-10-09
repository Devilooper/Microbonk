using Microbonk.Features.Collectibles.Runtime.Components.Homing;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Collectibles.Runtime.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(CollectibleRespondersGroup))]
    public partial struct CollectibleCounterSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var eventData in SystemAPI.Query<RefRO<CollectedEvent>>())
            {
            }
        }
    }
}