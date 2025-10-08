using Microbonk.Features.Collectibles.Runtime.Components.Homing;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Collectibles.Authoring
{
    [DisallowMultipleComponent]
    public class CollectibleHomingAuthoring : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float pickupRadius = 2;
        [SerializeField] [Min(0f)] private float completeRadius = 0.1f;
        [SerializeField] [Min(0f)] private float pickupAttractionSpeed = 10f;
        

        private class CollectibleHomingBaker : Baker<CollectibleHomingAuthoring>
        {
            public override void Bake(CollectibleHomingAuthoring authoring)
            {
                Entity collectibleSingletonEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(collectibleSingletonEntity, new CollectibleHomingSettingsSingleton
                {
                    Speed = authoring.pickupAttractionSpeed,
                    AcquireRadius = authoring.pickupRadius,
                    CompleteRadius = authoring.completeRadius
                });
            }
        }
    }
}