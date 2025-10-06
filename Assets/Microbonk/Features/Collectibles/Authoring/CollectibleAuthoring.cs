using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CollectibleAuthoring : MonoBehaviour
{
    [SerializeField] [Min(0)] private int value = 1;

    private sealed class CollectibleBaker : Baker<CollectibleAuthoring>
    {
        public override void Bake(CollectibleAuthoring authoring)
        {
            Entity collectibleEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<CollectibleTag>(collectibleEntity);
            AddComponent(collectibleEntity, new CollectibleValue
            {
                Amount = authoring.value
            });
        }
    }
}