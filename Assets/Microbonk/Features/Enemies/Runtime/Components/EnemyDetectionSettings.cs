using System;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;

namespace Microbonk.Features.Enemies.Runtime.Components
{
    [Serializable]
    public struct EnemyDetectionSettings : IComponentData   // todo ISharedComponentData
    {
        public float DetectionDistance;
        public CollisionFilter CollisionFilter;
        // public PhysicsCategoryTags EnemyBelongsTo;
        // public PhysicsCategoryTags TargetBelongsTo;
    }
}