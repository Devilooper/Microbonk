using Unity.Entities;

namespace Microbonk.Features.Collectibles.Runtime.Components
{
    public struct CollectibleDropSettingsSingleton : IComponentData
    {
        public Entity DropPrefab;
        public float DropRatePercent;
    }
}