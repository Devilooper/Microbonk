using Microbonk.Features.Collectibles.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Collectibles.Authoring
{
    [DisallowMultipleComponent]
    public sealed class CollectibleAuthoring : MonoBehaviour
    {
        private sealed class CollectibleBaker : Baker<CollectibleAuthoring>
        {
            public override void Bake(CollectibleAuthoring authoring)
            {
                Entity collectibleEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CollectibleTag>(collectibleEntity);
            }
        }
    }
}