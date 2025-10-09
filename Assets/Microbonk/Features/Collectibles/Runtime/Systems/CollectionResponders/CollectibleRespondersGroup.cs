using Microbonk.Features.Collectibles.Runtime.Systems;
using Unity.Entities;

namespace Microbonk.Features.Collectibles.Runtime
{
    [UpdateAfter(typeof(CollectibleHomingSystem))]
    [UpdateBefore(typeof(DestroyMarkedCollectiblesSystem))]
    public partial class CollectibleRespondersGroup : ComponentSystemGroup
    {
    }
}