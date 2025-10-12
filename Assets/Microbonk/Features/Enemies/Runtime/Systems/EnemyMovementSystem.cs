using Microbonk.Features.Enemies.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Microbonk.Features.Enemies.Runtime.Systems
{
    [BurstCompile]
    public partial struct EnemyMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.TempJob);

            foreach (var (characterControl, aiController, localTransform) in SystemAPI
                         .Query<RefRW<ThirdPersonCharacterControl>,
                             RefRO<EnemyDetectionSettings>,
                             RefRW<LocalTransform>>())
            {
                // Clear our detected hits list between each use
                distanceHits.Clear();

                // Create a hit collector for the detection hits
                AllHitsCollector<DistanceHit> hitsCollector =
                    new AllHitsCollector<DistanceHit>(aiController.ValueRO.DetectionDistance, ref distanceHits);

                // Detect hits that are within the detection range of the AI character
                PointDistanceInput distInput = new PointDistanceInput
                {
                    Position = localTransform.ValueRO.Position,
                    MaxDistance = aiController.ValueRO.DetectionDistance,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = CollisionFilter.Default.BelongsTo,
                        CollidesWith = aiController.ValueRO.DetectionFilter.Value
                    },
                };
                physicsWorld.CalculateDistance(distInput, ref hitsCollector);

                // Iterate on all detected hits to try to find a human-controlled character...
                Entity selectedTarget = Entity.Null;
                for (int i = 0; i < hitsCollector.NumHits; i++)
                {
                    Entity hitEntity = distanceHits[i].Entity;

                    // If it has a character component but no AIController component, that means it's a human player character
                    if (SystemAPI.HasComponent<ThirdPersonCharacterComponent>(hitEntity) &&
                        !SystemAPI.HasComponent<EnemyDetectionSettings>(hitEntity))
                    {
                        selectedTarget = hitEntity;
                        break; // early out
                    }
                }

                // In the character control component, set a movement vector that will make the ai character move towards the selected target
                if (selectedTarget != Entity.Null)
                {
                    characterControl.ValueRW.MoveVector = math.normalizesafe(
                        SystemAPI.GetComponent<LocalTransform>(selectedTarget).Position -
                        localTransform.ValueRO.Position);
                }
                else
                {
                    characterControl.ValueRW.MoveVector = float3.zero;
                }
            }
        }
    }
}