using Unity.Entities;
using Unity.Mathematics;

namespace Microbonk.Features.Collectibles.Runtime.Components.Homing
{
    public struct CollectedEvent : IComponentData
    {
        public Entity Collectible;  // this is unnecessary, as this component is on the same entity as the CollectibleTag
        public Entity Target;       // this could be renamed to CollectedBy
        public float3 Position;     // this is unnecessary, as this component is on the same entity as the LocalToWorld
    }
}