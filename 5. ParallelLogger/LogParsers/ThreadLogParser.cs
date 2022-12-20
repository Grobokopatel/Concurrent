using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LogParsing.LogParsers
{
    internal class ThreadLogParser : ILogParser
    {
        private readonly FileInfo file;
        private readonly Func<string, string?> tryGetIdFromLine;

        public ThreadLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            this.file = file;
            this.tryGetIdFromLine = tryGetIdFromLine;
        }

        public string[] GetRequestedIdsFromLogFile()
        {
            var lines = File.ReadAllLines(file.FullName);
            var lineCount = lines.Length;
            var processorCount = Environment.ProcessorCount;
            var threads = new Thread[processorCount];
            var tasksPerThread = (int)Math.Ceiling((double)lineCount / processorCount);
            for (var i = 0; i < processorCount; ++i)
            {
                var from = tasksPerThread * i;
                var to = Math.Min(tasksPerThread * (i + 1), lineCount);
                threads[i] = new Thread(ParseLogs);

                void ParseLogs()
                {
                    for (var j = from; j < to; ++j)
                        lines[j] = tryGetIdFromLine(lines[j]);
                }
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
            return lines.Where(l => l is not null)
                .ToArray();
        }
    }
}
