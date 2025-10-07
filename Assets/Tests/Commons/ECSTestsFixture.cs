using NUnit.Framework;
using Unity.Core;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.LowLevel;

// Based on https://github.com/needle-mirror/com.unity.entities/blob/master/Unity.Entities.Tests/ECSTestsFixture.cs
// and https://gamedev.center/unit-testing-made-easy-unity-ecs-best-practices/
public abstract class ECSTestsFixture
{
    protected World World;
    protected EntityManager Manager;

    private EntityManager.EntityManagerDebug ManagerDebug;
    private World PreviousWorld;
    private PlayerLoopSystem PreviousPlayerLoop;
    private bool JobsDebuggerWasEnabled;
    private double lastTickTime = 0;


    protected void UpdateSystem<T>() where T : unmanaged, ISystem =>
        this.World.GetExistingSystem<T>().Update(this.World.Unmanaged);

    protected SystemHandle CreateSystem<T>() where T : unmanaged, ISystem => this.World.CreateSystem<T>();

    protected Entity CreateEntity(params ComponentType[] types) => this.Manager.CreateEntity(types);


    /// <summary>
    ///     Useful if testing system that updates entities based on SystemAPI.Time.DeltaTime
    /// </summary>
    /// <param name="deltaTime"></param>
    protected void AdvanceWorldTime(float deltaTime = 0.2f)
    {
        this.World.SetTime(new TimeData(this.lastTickTime, deltaTime: deltaTime));
        this.lastTickTime += deltaTime;
    }

    [SetUp]
    public virtual void Setup()
    {
        // unit tests preserve the current player loop to restore later, and start from a blank slate.
        this.PreviousPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());

        this.PreviousWorld = World.DefaultGameObjectInjectionWorld;
        this.World = World.DefaultGameObjectInjectionWorld = new World("Test World");
        this.World.UpdateAllocatorEnableBlockFree = true;
        this.Manager = this.World.EntityManager;
        this.ManagerDebug = new EntityManager.EntityManagerDebug(this.Manager);

        // Many ECS tests will only pass if the Jobs Debugger enabled;
        // force it enabled for all tests, and restore the original value at teardown.
        this.JobsDebuggerWasEnabled = JobsUtility.JobDebuggerEnabled;
        JobsUtility.JobDebuggerEnabled = true;

        // JobsUtility.ClearSystemIds();

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !DISABLE_ENTITIES_JOURNALING
        // In case entities journaling is initialized, clear it
        EntitiesJournaling.Clear();
#endif
    }

    [TearDown]
    public virtual void TearDown()
    {
        if (this.World != null && this.World.IsCreated)
        {
            // Note that World.Dispose() already completes all jobs. But some tests may leave tests running when
            // they return, but we can't safely run an internal consistency check with jobs running, so we
            // explicitly complete them here as well.
            this.World.EntityManager.CompleteAllTrackedJobs();
            this.World.DestroyAllSystemsAndLogException(out bool errorsWhileDestroyingSystems);
            Assert.IsFalse(errorsWhileDestroyingSystems,
                "One or more exceptions were thrown while destroying systems during test teardown; consult the log for details.");

            this.ManagerDebug.CheckInternalConsistency();

            this.World.Dispose();
            this.World = null;

            World.DefaultGameObjectInjectionWorld = this.PreviousWorld;
            this.PreviousWorld = null;
            this.Manager = default;
        }

        JobsUtility.JobDebuggerEnabled = this.JobsDebuggerWasEnabled;
        // JobsUtility.ClearSystemIds();

        PlayerLoop.SetPlayerLoop(this.PreviousPlayerLoop);
    }
}