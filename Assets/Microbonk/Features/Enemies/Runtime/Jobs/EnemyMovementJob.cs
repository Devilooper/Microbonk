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
     
        }
    }
}