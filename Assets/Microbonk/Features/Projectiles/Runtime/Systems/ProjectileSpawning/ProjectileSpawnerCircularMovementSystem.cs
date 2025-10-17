using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Microbonk.Features.Projectiles.Runtime.Systems.ProjectileSpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Microbonk.Features.Projectiles.Runtime.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ProjectileSpawnerCircularMovementSystem : ISystem
    {
        [ReadOnly] private ComponentLookup<LocalToWorld> targetsPositions;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CircularMovementAroundTransform>();
            this.targetsPositions = state.GetComponentLookup<LocalToWorld>(isReadOnly: true);
        }

        public void OnUpdate(ref SystemState state)
        {
            this.targetsPositions.Update(ref state);

            foreach (var (spawner, spawnerTransform) in SystemAPI
                         .Query<
                             RefRO<CircularMovementAroundTransform>,
                             RefRW<LocalTransform>>())
            {
                var speed = spawner.ValueRO.AngularSpeed;
                var radius = spawner.ValueRO.Radius;
                var origin = this.targetsPositions[spawner.ValueRO.Origin].Position;

                var time = SystemAPI.Time.ElapsedTime;
                var angle = (float)time * speed;

                spawnerTransform.ValueRW.Position = new float3(
                    origin.x + math.sin(angle) * radius,
                    origin.y + 0,
                    origin.z + math.cos(angle) * radius
                );

                var direction = new float3(math.cos(angle - math.PIHALF), 0, math.sin(angle+ math.PIHALF));
                spawnerTransform.ValueRW.Rotation = quaternion.LookRotation(direction, math.up());
            }
        }
    }
}