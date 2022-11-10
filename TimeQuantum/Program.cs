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
            

            var firstThread = new Thread(() => { while (CancellationToken) { } })
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal,
            };
            

            var secondThread = new Thread(() => { CancellationToken = false; })
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal,
            };
            var stopWatch = new Stopwatch();

            firstThread.Start();
            secondThread.Start();
            stopWatch.Start();

            secondThread.Join();
            stopWatch.Stop();

            Console.WriteLine(stopWatch.ElapsedMilliseconds);
            firstThread.Join();
        }
    }
}