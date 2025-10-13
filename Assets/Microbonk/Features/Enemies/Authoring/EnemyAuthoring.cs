using Microbonk.Features.Enemies.Runtime.Components;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;

namespace Microbonk.Features.Enemies.Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float DetectionDistance;
        [SerializeField] private PhysicsShapeAuthoring EnemyPhysicsShape;
        [SerializeField] private PhysicsCategoryTags TargetBelongsTo;

        private class EnemyAuthoringBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddSharedComponent(entity, new EnemyDetectionSettings
                {
                    DetectionDistance = authoring.DetectionDistance,
                    CollisionFilter = new CollisionFilter
                    {
                        BelongsTo = authoring.EnemyPhysicsShape.BelongsTo.Value,
                        CollidesWith = authoring.TargetBelongsTo.Value
                    }
                });
            }
        }
    }
}