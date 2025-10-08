using Unity.Entities;
using Unity.Mathematics;

namespace Microbonk.Features.Collectibles.Runtime.Components.Homing
{
    public struct CollectedEvent : IComponentData
    {
        public Entity Collectible;
        public Entity Target;
        public float3 Position;
    }
}