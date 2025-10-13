using Microbonk.Features.Enemies.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Microbonk.Features.Enemies.Runtime.Jobs
{
    [BurstCompile]
    public partial struct EnemyMovementJob : IJobEntity
    {
        [ReadOnly] public PhysicsWorld PhysicsWorld;

        public void Execute(ref ThirdPersonCharacterControl  characterControl, in EnemyDetectionSettings detectionSettings, in  LocalTransform localTransform )
        {
            NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
            
            AllHitsCollector<DistanceHit> hitsCollector =
                new AllHitsCollector<DistanceHit>(detectionSettings.DetectionDistance, ref distanceHits);
            
            PointDistanceInput distInput = new PointDistanceInput
            {
                Position = localTransform.Position,
                MaxDistance = detectionSettings.DetectionDistance,
                Filter = new CollisionFilter
                {
                    BelongsTo = CollisionFilter.Default.BelongsTo,
                    CollidesWith = detectionSettings.DetectionFilter.Value
                },
            };
            
            this.PhysicsWorld.CalculateDistance(distInput, ref hitsCollector);
        }
    }
}