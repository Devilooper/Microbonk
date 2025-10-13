using Microbonk.Features.Player.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Enemies.Authoring
{
    public class EnemyMovementTargetAuthoring : MonoBehaviour
    {
        private class EnemyMovementTargetBaker : Baker<EnemyMovementTargetAuthoring>
        {
            public override void Bake(EnemyMovementTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<EnemyMovementTargetTag>(entity);
            }
        }
    }
}