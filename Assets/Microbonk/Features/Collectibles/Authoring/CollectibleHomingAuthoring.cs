using Game.Features.Collectibles.Runtime.Components.Homing;
using Unity.Entities;
using UnityEngine;

namespace Game.Features.Collectibles.Authoring
{
    [DisallowMultipleComponent]
    public class CollectibleHomingAuthoring : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float pickupRadius = 2;
        [SerializeField] [Min(0f)] private float pickupAttractionSpeed = 10f;

        private class CollectibleHomingBaker : Baker<CollectibleHomingAuthoring>
        {
            public override void Bake(CollectibleHomingAuthoring authoring)
            {
                Entity collectibleSingletonEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(collectibleSingletonEntity, new CollectibleHomingRadius
                {
                    HomingRadius = authoring.pickupRadius
                });
                AddComponent(collectibleSingletonEntity, new CollectibleHomingSpeed
                {
                    HomingSpeed = authoring.pickupAttractionSpeed
                });
            }
        }
    }
}