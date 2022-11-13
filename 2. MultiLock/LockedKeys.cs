using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLock
{
    public class LockedKeys<T> : IDisposable
        where T : class
    {
        private readonly IEnumerable<T> _keys;
        public LockedKeys(IEnumerable<T> keys)
        {
            _keys = keys;
        }

        public void Dispose()
        {
            foreach (var k in _keys)
                Monitor.Exit(k);
        }
    }
}
