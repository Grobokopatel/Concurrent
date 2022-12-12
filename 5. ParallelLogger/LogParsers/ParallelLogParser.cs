using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParsing.LogParsers
{
    internal class ParallelLogParser : ILogParser
    {
        private readonly FileInfo file;
        private readonly Func<string, string?> tryGetIdFromLine;

        public ParallelLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            this.file = file;
            this.tryGetIdFromLine = tryGetIdFromLine;
        }

        public string[] GetRequestedIdsFromLogFile()
        {
            var lines = File.ReadLines(file.FullName);
            var result = new ConcurrentStack<string>();
            Parallel.ForEach(lines, l =>
            {
                var log = tryGetIdFromLine(l);
                if (log is not null)
                    result.Push(log);
            });

            return result.ToArray();
        }
    }
}
