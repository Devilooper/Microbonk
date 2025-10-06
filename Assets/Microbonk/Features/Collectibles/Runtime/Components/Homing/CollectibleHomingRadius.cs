using Unity.Entities;

namespace Game.Features.Collectibles.Runtime.Components.Homing
{
    /// <summary>
    ///     Denotes minimum distance to player for collectible to be picked up
    /// </summary>
    public struct CollectibleHomingRadius : IComponentData
    {
        public float HomingRadius;
    }
}