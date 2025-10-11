using Microbonk.Features.Projectiles.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Projectiles.Authoring
{
    [DisallowMultipleComponent]
    public class ProjectileAuthoring : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] [Min(0f)] private float lifetime = 10f;

        private class ProjectileBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ProjectileSpeed
                {
                    Speed = authoring.speed,
                });

                if (authoring.lifetime > 0)
                {
                    AddComponent(entity, new ProjectileLifetime
                    {
                        SecondsRemaining = authoring.lifetime
                    });
                }
            }
        }
    }
}