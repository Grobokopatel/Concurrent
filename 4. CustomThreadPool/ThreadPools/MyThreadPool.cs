using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomThreadPool.ThreadPools
{
    internal class MyThreadPool : IThreadPool
    {
        internal readonly ConcurrentQueue<Action> globalQueue;
        internal readonly Worker[] workers;
        internal volatile int waitingThreadsCount;
        internal long tasksProcessedCount;

        public MyThreadPool(int workerCount)
        {
            globalQueue = new ConcurrentQueue<Action>();
            workers = new Worker[workerCount];
            for (var i = 0; i < workerCount; ++i)
                workers[i] = new Worker(this);
            for (var i = 0; i < workerCount; ++i)
                workers[i].Start();
        }

        public MyThreadPool() : this(Environment.ProcessorCount) { }

        public void EnqueueAction(Action action)
        {
            globalQueue.Enqueue(action);

            if (waitingThreadsCount > 0)
            {
                lock (globalQueue)
                {
                    Monitor.Pulse(globalQueue);
                }
            }
        }

        public long GetTasksProcessedCount() => tasksProcessedCount;
    }
}
