using Unity.Entities;

namespace Microbonk.Features.Projectiles.Runtime.Components.ProjectileSpawning
{
    public struct CircularMovementAroundTransform : IComponentData
    {
        public Entity Origin;
        public float Radius;
        public float AngularSpeed;
    }
}