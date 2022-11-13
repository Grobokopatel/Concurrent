using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLock
{
    public interface IMultiLock
    {
        public IDisposable AcquireLock(params string[] keys);
    }
}
