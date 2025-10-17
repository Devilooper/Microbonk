using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Microbonk.Features.Projectiles.Runtime.Systems
{
    [BurstCompile]
    public partial struct CircularMovementJob : IJobEntity
    {
        public float ElapsedTime;

        [ReadOnly]
        public ComponentLookup<LocalToWorld> OriginPositions;

        private void Execute(ref LocalTransform spawnerTransform, in CircularMovementAroundTransform spawner)
        {
            var speed = spawner.AngularSpeed;
            var radius = spawner.Radius;
            var origin = this.OriginPositions[spawner.Origin].Position;
            var angle = this.ElapsedTime * speed;

            var position = new float3(
                origin.x + math.sin(angle) * radius,
                origin.y,
                origin.z + math.cos(angle) * radius
            );

            var direction = new float3(
                math.cos(angle - math.PIHALF),
                0,
                math.sin(angle + math.PIHALF));
            var rotation = quaternion.LookRotation(direction, math.up());
            
            spawnerTransform = LocalTransform.FromPositionRotation(position, rotation);
        }
    }
}
