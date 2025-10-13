using Microbonk.Features.Enemies.Runtime.Components;
using Microbonk.Features.Player.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

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
            NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach (var (enemyControl, detectionSettings, enemyTransform) in SystemAPI
                         .Query<RefRW<ThirdPersonCharacterControl>,
                             RefRO<EnemyDetectionSettings>,
                             RefRO<LocalTransform>>())
            {
                // Clear our detected hits list between each use
                distanceHits.Clear();

                // Create a hit collector for the detection hits
                AllHitsCollector<DistanceHit> hitsCollector =
                    new AllHitsCollector<DistanceHit>(detectionSettings.ValueRO.DetectionDistance, ref distanceHits);

                // Detect hits that are within the detection range of the AI character
                PointDistanceInput distInput = new PointDistanceInput
                {
                    Position = enemyTransform.ValueRO.Position,
                    MaxDistance = detectionSettings.ValueRO.DetectionDistance,
                    Filter = detectionSettings.ValueRO.CollisionFilter
                };
                physicsWorld.CalculateDistance(distInput, ref hitsCollector);

                Entity selectedTarget = Entity.Null;
                float closestDistanceSq = float.MaxValue;

                for (int i = 0; i < hitsCollector.NumHits; i++)
                {
                    float distSq = math.distancesq(enemyTransform.ValueRO.Position, distanceHits[i].Position);
                    if (distSq < closestDistanceSq)
                    {
                        closestDistanceSq = distSq;
                        selectedTarget = distanceHits[i].Entity;
                    }
                }


                // In the character control component, set a movement vector that will make the ai character move towards the selected target
                if (selectedTarget != Entity.Null)
                {
                    enemyControl.ValueRW.MoveVector = math.normalizesafe(
                        SystemAPI.GetComponent<LocalTransform>(selectedTarget).Position -
                        enemyTransform.ValueRO.Position);
                }
                else
                {
                    enemyControl.ValueRW.MoveVector = float3.zero;
                }
            }
        }
    }
}