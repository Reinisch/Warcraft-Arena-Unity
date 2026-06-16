using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    /// <summary>
    /// Limits concurrent access to a child node using a shared <see cref="TaskGuardLock"/> asset.
    /// Equivalent to Behavior Designer's TaskGuard decorator.
    ///
    /// When the lock is at capacity:
    ///   - WaitUntilAvailable = true  → the modifier keeps running (polls each frame) until a slot opens.
    ///   - WaitUntilAvailable = false → the modifier immediately returns Failure, skipping the child.
    ///
    /// Multiple modifier nodes referencing the same lock asset share the same slot counter,
    /// replacing Behavior Designer's LinkedTask links.
    /// </summary>
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Task Guard",
        description: "Limits concurrent access to its child using a shared lock. Waits or skips when the lock is at capacity.",
        story: "Guard with [Lock], wait if unavailable: [WaitUntilAvailable]",
        category: "Flow",
        id: "f2a3b4cad6e7f8a9b0c1d2e3f4a5b6ca")]
    public class TaskGuardModifier : Modifier
    {
        [SerializeReference] public BlackboardVariable<int> MaxLockAccessCount = new(1);
        [SerializeReference] public BlackboardVariable<bool> WaitUntilAvailable = new(true);
        [SerializeReference] public BlackboardVariable<TaskGuardLock> Lock;

        [CreateProperty] private bool acquired;
        [CreateProperty] private bool createdLock;

        protected override void OnSetup()
        {
            base.OnSetup();

            if (Lock.Value == null)
            {
                createdLock = true;
                Lock.Value = ScriptableObject.CreateInstance<TaskGuardLock>();
                Lock.Value.MaxAccessCount = MaxLockAccessCount;
            }
        }

        protected override void OnTeardown()
        {
            base.OnTeardown();

            if (Lock.Value != null && createdLock)
            {
                Lock.Value = ScriptableObject.CreateInstance<TaskGuardLock>();
                Object.Destroy(Lock.Value);
                Lock.Value = null;
                createdLock = false;
            }
        }

        protected override Status OnStart()
        {
            acquired = false;

            if (Lock.Value != null && !Lock.Value.TryAcquire())
                return WaitUntilAvailable.Value ? Status.Running : Status.Failure;

            return AcquireAndStartChild();
        }

        protected override Status OnUpdate()
        {
            if (!acquired)
            {
                // Polling for an available slot each frame.
                if (Lock.Value != null && !Lock.Value.TryAcquire())
                    return Status.Running;

                return AcquireAndStartChild();
            }

            // Slot is held — forward the child's status upward.
            Status childStatus = Child != null ? Child.CurrentStatus : Status.Success;
            return childStatus == Status.Running ? Status.Waiting : childStatus;
        }

        protected override void OnEnd()
        {
            if (acquired && Lock.Value != null)
            {
                Lock.Value.Release();
                acquired = false;
            }
        }

        private Status AcquireAndStartChild()
        {
            acquired = true;

            if (Child == null)
                return Status.Success;

            Status childStatus = StartNode(Child);
            return childStatus == Status.Running ? Status.Waiting : childStatus;
        }
    }
}
