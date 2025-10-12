using System;
using Unity.Entities;
using Unity.Physics.Authoring;

namespace Microbonk.Features.Enemies.Runtime.Components
{
    [Serializable]
    public struct EnemyDetectionSettings : IComponentData
    {
        public float DetectionDistance;
        public PhysicsCategoryTags DetectionFilter;
    }
}