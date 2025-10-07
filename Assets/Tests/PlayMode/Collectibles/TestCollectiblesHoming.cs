using Game.Features.Collectibles.Runtime.Components.Homing;
using Game.Features.Collectibles.Runtime.Systems;
using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TestCollectiblesHoming : ECSTestsFixture
{
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        CreateSystem<CollectibleHomingSystem>();
    }

    [Test]
    public void TestCollectibleShouldMoveWhenHomingTargetIsInRange()
    {
        // Arrange
        Entity collectible = CreateCollectible(position: new float3(1, 1, 1));
        CreateHomingTarget(position: new float3(2, 2, 2));

        // Act
        var initialTransform = this.Manager.GetComponentData<LocalTransform>(collectible);
        AdvanceWorldTime();
        UpdateSystem<CollectibleHomingSystem>();
        var updatedTransform = this.Manager.GetComponentData<LocalTransform>(collectible);

        // Assert
        Assert.AreNotEqual(initialTransform.Position, updatedTransform.Position);
    }

    [Test]
    public void TestCollectibleShouldNotMoveWhenHomingTargetIsOutsideRange()
    {
        // Arrange
        Entity collectible = CreateCollectible(position: new float3(1, 1, 1));
        CreateHomingTarget(position: new float3(20, 20, 20));

        // Act
        var initialTransform = this.Manager.GetComponentData<LocalTransform>(collectible);
        AdvanceWorldTime();
        UpdateSystem<CollectibleHomingSystem>();
        var updatedTransform = this.Manager.GetComponentData<LocalTransform>(collectible);

        // Assert
        Assert.AreEqual(initialTransform.Position, updatedTransform.Position);
    }

    private void CreateHomingTarget(float3 position)
    {
        Entity target = this.Manager.CreateEntity(typeof(HomingTargetTag));
        this.Manager.AddComponentData(target, new LocalTransform { Position = position });
    }

    private Entity CreateCollectible(float3 position)
    {
        Entity collectible = this.Manager.CreateEntity(typeof(CollectibleTag));
        this.Manager.AddComponentData(collectible, new LocalTransform { Position = position });
        this.Manager.AddComponentData(collectible,
            new CollectibleHomingSettingsSingleton { AcquireRadius = 10, Speed = 1 });
        return collectible;
    }
}