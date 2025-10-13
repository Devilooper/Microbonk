using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;

namespace Microbonk.Features.Enemies.Runtime.Components
{
    [Serializable]
    [BurstCompile]
    public struct EnemyDetectionSettings : ISharedComponentData, IEquatable<EnemyDetectionSettings>
    {
        public float DetectionDistance;
        public CollisionFilter CollisionFilter;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EnemyDetectionSettings other)
        {
            return this.DetectionDistance.Equals(other.DetectionDistance) &&
                   this.CollisionFilter.Equals(other.CollisionFilter);
        }

        [BurstCompile]
        public override int GetHashCode()
        {
            return HashCode.Combine(this.DetectionDistance, this.CollisionFilter);
        }
    }
}