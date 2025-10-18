using Microbonk.Features.Collectibles.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Collectibles.Authoring
{
    public class CollectibleDropSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject DropPrefab;
        [Range(0f, 1f)] [SerializeField] private float DropRatePercent;

        private class CollectibleDropSettingsBaker : Baker<CollectibleDropSettingsAuthoring>
        {
            public override void Bake(CollectibleDropSettingsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CollectibleDropSettingsSingleton
                {
                    DropPrefab = GetEntity(authoring.DropPrefab, TransformUsageFlags.Dynamic),
                    DropRatePercent = authoring.DropRatePercent
                });
            }
        }
    }
}