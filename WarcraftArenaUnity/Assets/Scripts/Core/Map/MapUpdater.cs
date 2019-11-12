using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Core
{
    public class MapUpdater
    {
        private class MapUpdateRequest
        {
            private readonly Map map;
            private readonly MapUpdater updater;
            private readonly int diff;

            public MapUpdateRequest(Map m, MapUpdater u, int d)
            {
                map = m;
                updater = u;
                diff = d;
            }

            public void Call()
            {
                map.DoUpdate(diff);
                updater.UpdateFinished();
            }
        }

        private readonly BlockingCollection<MapUpdateRequest> updateRequests = new BlockingCollection<MapUpdateRequest>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly List<Thread> workerThreads = new List<Thread>();

        private int pendingRequests;

        public bool Activated => workerThreads.Count > 0;

        public MapUpdater()
        {
            pendingRequests = 0;
        }

        public void Activate(int threads)
        {
            for (int i = 0; i < threads; ++i)
            {
                var newThread = new Thread(WorkerThread);
                workerThreads.Add(newThread);

                newThread.Start();
            }
        }

        public void Deactivate()
        {
            cancellationTokenSource.Cancel();

            Wait();

            updateRequests.Dispose();
            workerThreads.ForEach(thread => thread.Join());
        }

        public void ScheduleUpdate(Map map, int deltaTime)
        {
            Interlocked.Increment(ref pendingRequests);

            updateRequests.Add(new MapUpdateRequest(map, this, deltaTime), cancellationTokenSource.Token);
        }

        public void Wait()
        {
            while (pendingRequests == 0)
                return;
        }  

        private void UpdateFinished()
        {
            Interlocked.Decrement(ref pendingRequests);
        }

        private void WorkerThread()
        {
            while (true)
            {
                if (updateRequests.TryTake(out MapUpdateRequest request, 10, cancellationTokenSource.Token))
                    request.Call();

                if (cancellationTokenSource.IsCancellationRequested)
                    return;
            }
        }
    }
}