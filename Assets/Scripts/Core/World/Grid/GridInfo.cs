namespace Core
{
    public class GridInfo
    {
        public TimeTracker Timer { get; private set; }
        public PeriodicTimer VisUpdate { get; private set; }
        public bool UnloadExplicitLock { get; set; }                // explicit manual lock or config setting
        public bool UnloadReferenceLock { get; set; }               // lock from instance map copy

        public bool UnloadLock => unloadActiveLockCount > 0 || UnloadExplicitLock || UnloadReferenceLock;

        private int unloadActiveLockCount;                          // lock from active object spawn points (prevent clone loading)


        public GridInfo()
        {
            Timer = new TimeTracker(0);
            VisUpdate = new PeriodicTimer(0, RandomHelper.Next(0, GridHelper.DefaultVisibilityNotifyPeriod));
            unloadActiveLockCount = 0;
            UnloadExplicitLock = false;
            UnloadReferenceLock = false;
        }

        public GridInfo(long expiry, bool unload = true)
        {
            Timer = new TimeTracker(expiry);
            VisUpdate = new PeriodicTimer(0, RandomHelper.Next(0, GridHelper.DefaultVisibilityNotifyPeriod));
            unloadActiveLockCount = 0;
            UnloadExplicitLock = !unload;
            UnloadReferenceLock = false;
        }


        public void IncUnloadActiveLock() { ++unloadActiveLockCount; }
        public void DecUnloadActiveLock() { if (unloadActiveLockCount > 0) --unloadActiveLockCount; }

        public void SetTimer(long expiery) { Timer.Reset(expiery); }
        public void ResetTimeTracker(long interval) { Timer.Reset(interval); }
        public void UpdateTimeTracker(long diff) { Timer.Update(diff); }
    }
}