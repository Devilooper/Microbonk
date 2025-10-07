using Unity.Entities;

namespace Game.Features.Collectibles.Runtime.Components.Homing
{
    public struct CollectibleHomingSettingsSingleton : IComponentData
    {
        public float Speed;
        public float AcquireRadius;
        public float CompleteRadius; // can be float.Epsilon
        
        public void Deconstruct(out float speed, out float acquireRadius, out float completeRadius)
        {
            speed = this.Speed;
            acquireRadius = this.AcquireRadius;
            completeRadius = this.CompleteRadius;
        }
    }
}