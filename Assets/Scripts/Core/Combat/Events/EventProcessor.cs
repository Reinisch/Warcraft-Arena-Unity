using System.Collections.Generic;

namespace Core
{
    public class EventProcessor
    {
        protected long Time { get; set; }
        protected Dictionary<long, List<BasicEvent>> Events { get; set; }

        public EventProcessor()
        {
            Events = new Dictionary<long, List<BasicEvent>>();
            Time = 0;
        }


        public void Update(int pTime) { }

        public void KillAllEvents(bool force) { }

        public void AddEvent(BasicEvent Event, long executionTime, bool setAddtime = true) { }

        public long CalculateTime(long offset) { return 0; }
    }
}