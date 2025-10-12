using Microbonk.Features.Enemies.Runtime.Components;
using UnityEngine;
using Unity.Entities;

namespace Microbonk.Features.Enemies.Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public EnemyDetectionSettings EnemyDetectionSettings;

        private class EnemyAuthoringBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, authoring.EnemyDetectionSettings);
            }
        }
    }
}