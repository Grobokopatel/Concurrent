using System.Diagnostics;

namespace TimeQuantum
{
    class Program
    {
        private static int _lastThreadId = Environment.CurrentManagedThreadId;
        private static Stopwatch _timer = new Stopwatch();
        private static List<long> _timeStamps = new List<long>();
        public static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << (Environment.ProcessorCount - 1));

            var thread1 = new Thread(WriteToTimeStamps);
            var thread2 = new Thread(WriteToTimeStamps);
            _timer.Start();
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
            _timer.Stop();

            Console.WriteLine(_timeStamps.Skip(3).Zip(_timeStamps.Skip(4), (p, c) => c - p).Average());
        }

        public static void WriteToTimeStamps()
        {
            for (long i = _timer.ElapsedMilliseconds; i < 1000; i = _timer.ElapsedMilliseconds)
            {
                if (_lastThreadId != Environment.CurrentManagedThreadId)
                {
                    _lastThreadId = Environment.CurrentManagedThreadId;
                    _timeStamps.Add(i);
                }
            }
        }
    }
}