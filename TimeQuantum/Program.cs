using System.Diagnostics;

namespace TimeQuantum
{
    class Program
    {
        private static bool CancellationToken;

        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << (Environment.ProcessorCount - 1));

            CancellationToken = true;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var firstThread = new Thread(() => { while (CancellationToken) { } })
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal,
            };
            firstThread.Start();

            var secondThread = new Thread(() => { CancellationToken = false; })
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal,
            };
            secondThread.Start();

            firstThread.Join();
            secondThread.Join();
            stopWatch.Stop();

            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }
    }
}