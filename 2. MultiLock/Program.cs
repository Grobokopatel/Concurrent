using System.Diagnostics;
using System.Text;

namespace MultiLock
{
    class Program
    {
        static void Main()
        {
            Demo();
        }

        public static void Demo()
        {
            var multiLock = new MyMultiLock();
            Thread thread1;
            Thread thread2;
            using (var locks = multiLock.AcquireLock("a", "b", "c"))
            {
                thread1 = new Thread(() =>
                {
                    using (var d2 = multiLock.AcquireLock("d", "e", "a"))
                    {
                        Console.WriteLine("Sleeping 1 second in thread1");
                        Thread.Sleep(1000);
                    }
                });

                thread2 = new Thread(() =>
                {
                    using (var d2 = multiLock.AcquireLock("f", "g", "a"))
                    {
                        Console.WriteLine("Sleeping 1 second in thread2");
                        Thread.Sleep(1000);
                    }
                });

                thread1.Start();
                thread2.Start();
                Console.WriteLine("Sleeping 3 seconds in main thread");
                Thread.Sleep(3000);
            }
            thread1.Join();
            thread2.Join();
        }
    }
}