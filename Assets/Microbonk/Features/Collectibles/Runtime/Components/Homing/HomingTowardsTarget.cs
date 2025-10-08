using Unity.Entities;

// This can't be inside a namespace as Burst errors out
public struct HomingTowardsTarget : IComponentData
{
    public Entity Target;
}