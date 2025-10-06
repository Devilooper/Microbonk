using Game.Features.Collectibles.Runtime.Components.Homing;
using Unity.Entities;
using UnityEngine;

namespace Game.Features.Collectibles.Authoring
{
    public class HomingTargetAuthoring : MonoBehaviour
    {
        private class HomingTargetBaker : Baker<HomingTargetAuthoring>
        {
            public override void Bake(HomingTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CollectibleHomingTargetTag());
            }
        }
    }
}