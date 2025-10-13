using Microbonk.Features.Enemies.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Microbonk.Features.Enemies.Runtime.Jobs
{
    [BurstCompile]
    public partial struct EnemyMovementJob : IJobEntity
    {
        [ReadOnly] public PhysicsWorld PhysicsWorld;
        [ReadOnly] public ComponentLookup<LocalTransform> TargetTransformLookup;

        private void Execute(ref ThirdPersonCharacterControl enemyControl,
            in EnemyDetectionSettings detectionSettings,
            in LocalTransform enemyTransform)
        {
            var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
            var hitsCollector =
                new AllHitsCollector<DistanceHit>(detectionSettings.DetectionDistance, ref distanceHits);

            PointDistanceInput distInput = new PointDistanceInput
            {
                Position = enemyTransform.Position,
                MaxDistance = detectionSettings.DetectionDistance,
                Filter = detectionSettings.CollisionFilter
            };
            this.PhysicsWorld.CalculateDistance(distInput, ref hitsCollector);

            Entity selectedTarget = Entity.Null;
            float closestDistanceSq = float.MaxValue;
            for (int i = 0; i < hitsCollector.NumHits; i++)
            {
                float distSq = math.distancesq(enemyTransform.Position, distanceHits[i].Position);
                if (distSq < closestDistanceSq)
                {
                    closestDistanceSq = distSq;
                    selectedTarget = distanceHits[i].Entity;
                }
            }

            if (selectedTarget != Entity.Null)
            {
                enemyControl.MoveVector = math.normalizesafe(
                    this.TargetTransformLookup[selectedTarget].Position - enemyTransform.Position);
            }
            else
            {
                enemyControl.MoveVector = float3.zero;
            }
        }
    }
}