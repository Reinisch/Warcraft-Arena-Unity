using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// A shared semaphore asset used by <see cref="TaskGuardModifier"/> nodes to limit
    /// concurrent access to a guarded child. Multiple modifier nodes referencing the same
    /// lock asset share the same access count, replacing Behavior Designer's LinkedTask links.
    /// </summary>
    [UsedImplicitly, CreateAssetMenu(fileName = "Task Guard Lock", menuName = "Game Data/AI/Task Guard Lock")]
    public class TaskGuardLock : ScriptableObject
    {
        [field: SerializeField, UsedImplicitly]
        public int MaxAccessCount { get; set; } = 1;

        private int currentCount;

        // Reset the counter whenever the asset is enabled (i.e. on play-mode entry).
        private void OnEnable() => currentCount = 0;

        /// <summary>
        /// Attempts to acquire one slot. Returns true and increments the counter if a slot is
        /// available; returns false without changing state if the lock is at capacity.
        /// </summary>
        public bool TryAcquire()
        {
            if (currentCount >= MaxAccessCount)
                return false;

            currentCount++;
            return true;
        }

        /// <summary>
        /// Releases one previously acquired slot.
        /// </summary>
        public void Release()
        {
            if (currentCount > 0)
                currentCount--;
        }
    }
}
