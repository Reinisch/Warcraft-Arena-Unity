namespace Core
{
    public class BasicEvent
    {
        private enum AbortState : byte
        {
            Running,
            AbortScheduled,
            Aborted
        }

        private AbortState State { get; set; } // set by externals when the event is aborted, aborted events don't execute
        private long AddTime { get; set; } // time when the event was added to queue, filled by event handler
        private long ExecTime { get; set; } // planned time of next execution, filled by event handler

        public BasicEvent()
        {
            State = AbortState.Running;
            AddTime = 0;
            ExecTime = 0;
        }

        // this method executes when the event is triggered
        // return false if event does not want to be deleted
        public virtual bool Execute(long executionTime, uint updateInterval)
        {
            return true;
        }

        // this event can be safely deleted
        public virtual bool IsDeletable()
        {
            return true;
        }

        // this method executes when the event is aborted
        public virtual void Abort(long executionTime)
        {
        }

        // Aborts the event at the next update tick
        public void ScheduleAbort()
        {
        }


        private void SetAborted()
        {
        }

        private bool IsRunning()
        {
            return State == AbortState.Running;
        }

        private bool IsAbortScheduled()
        {
            return State == AbortState.AbortScheduled;
        }

        private bool IsAborted()
        {
            return State == AbortState.Aborted;
        }
    }
}