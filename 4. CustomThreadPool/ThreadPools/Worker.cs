using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomThreadPool.ThreadPools
{
    internal class Worker
    {
        private readonly WorkStealingQueue<Action> _localQueue;
        private readonly Thread _thread;
        private readonly MyThreadPool _parentThreadPool;

        public Worker(MyThreadPool parentThreadPool)
        {
            _localQueue = new WorkStealingQueue<Action>();
            _parentThreadPool = parentThreadPool;
            _thread = new Thread(WorkLoop) { IsBackground = true };
        }

        public void Start()
        {
            _thread.Start();
        }

        private void WorkLoop()
        {
            while (true)
            {
                GetTask()();
                Interlocked.Increment(ref _parentThreadPool.tasksProcessedCount);
            }
        }

        private Action GetTask()
        {
            if (TryGetTaskFromLocalQueue(out var task))
                return task;

            while (true)
            {
                if (TryGetTaskFromGlobalQueue(out task))
                    return task;

                if (TryStealTask(out task))
                    return task;

                WaitForNewTask();
            }
        }

        private bool TryGetTaskFromLocalQueue(out Action task)
        {
            task = null;
            return _localQueue.LocalPop(ref task);
        }

        private bool TryGetTaskFromGlobalQueue(out Action task)
        {
            return _parentThreadPool.globalQueue.TryDequeue(out task);
        }

        private bool TryStealTask(out Action task)
        {
            task = null;
            foreach (var otherWorker in _parentThreadPool.workers)
            {
                if (otherWorker != this)
                {
                    if (otherWorker._localQueue.TrySteal(ref task))
                        return true;
                }
            }

            return false;
        }

        internal void WaitForNewTask()
        {
            var globalQueue = _parentThreadPool.globalQueue;
            ref int waitingThreadCount = ref _parentThreadPool.waitingThreadsCount;
            lock (globalQueue)
            {
                ++waitingThreadCount;
                try
                {
                    Monitor.Wait(globalQueue);
                }
                finally
                {
                    --waitingThreadCount;
                }
            }
        }
    }
}
