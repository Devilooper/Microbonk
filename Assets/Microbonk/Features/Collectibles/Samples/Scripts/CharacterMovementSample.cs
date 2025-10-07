using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Microbonk.Features.Collectibles.Samples.Scripts
{
    public class CharacterMovementSampleAuthoring : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private float radius = 5f;

        public class CharacterMovementSampleBaker : Baker<CharacterMovementSampleAuthoring>
        {
            public override void Bake(CharacterMovementSampleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CharacterMovementSampleComponentData
                {
                    Speed = authoring.speed,
                    Radius = authoring.radius,
                    Center = authoring.transform.position
                });
            }
        }
    }

    public struct CharacterMovementSampleComponentData : IComponentData
    {
        public float3 Center;
        public float Speed;
        public float Radius;
    }


    [BurstCompile]
    [WithAll(typeof(CharacterMovementSampleComponentData))]
    public partial struct CharacterMovementSampleSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (characterMovementSampleComponentData, localTransform) in SystemAPI
                         .Query<RefRO<CharacterMovementSampleComponentData>, RefRW<LocalTransform>>())
            {
                var speed = characterMovementSampleComponentData.ValueRO.Speed;
                var radius = characterMovementSampleComponentData.ValueRO.Radius;
                var center = characterMovementSampleComponentData.ValueRO.Center;

                var time = SystemAPI.Time.ElapsedTime;
                var angle = (float)time * speed;

                localTransform.ValueRW.Position = new float3(
                    center.x + math.sin(angle) * radius,
                    center.y + 0,
                    center.z + math.cos(angle) * radius
                );
            }
        }
    }
}