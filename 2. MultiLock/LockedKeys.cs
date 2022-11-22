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
        private readonly string[] _keys;
        public LockedKeys(string[] keys)
        {
            _keys = keys;
        }

        public void Dispose()
        {
            for(var i=_keys.Length-1; i>=0; --i)
                Monitor.Exit(_keys[i]);
        }
    }
}
