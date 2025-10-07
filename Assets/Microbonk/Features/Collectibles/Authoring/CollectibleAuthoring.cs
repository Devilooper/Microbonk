using Unity.Entities;
using UnityEngine;

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