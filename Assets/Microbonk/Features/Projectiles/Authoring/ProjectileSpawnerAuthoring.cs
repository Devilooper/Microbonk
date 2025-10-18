using Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning;
using Unity.Entities;
using UnityEngine;

namespace Microbonk.Features.Projectiles.Authoring
{
    public class ProjectileSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private Transform RotateAround;
        [SerializeField] private GameObject ProjectileToSpawn;
        [SerializeField] private float SpawnerAngularSpeed;
        [SerializeField] private float SpawnerRadius;
        [SerializeField] private float IntervalSeconds;

        private class ProjectileSpawnerBaker : Baker<ProjectileSpawnerAuthoring>
        {
            public override void Bake(ProjectileSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CircularMovementAroundTransform
                {
                    Origin = GetEntity(authoring.RotateAround, TransformUsageFlags.Dynamic),
                    AngularSpeed = authoring.SpawnerAngularSpeed,
                    Radius = authoring.SpawnerRadius
                });
                AddComponent(entity, new ProjectileSpawner
                {
                    ToSpawn = GetEntity(authoring.ProjectileToSpawn, TransformUsageFlags.Dynamic),
                    SpawnIntervalSeconds = authoring.IntervalSeconds
                });
                AddComponent(entity, new SpawnerCooldown
                {
                    SecondsRemaining = authoring.IntervalSeconds
                });
            }
        }
    }
}