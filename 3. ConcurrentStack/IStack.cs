using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentStack
{
    public interface IStack<T>
    {
        void Push(T item);
        bool TryPop(out T item);
        int Count { get; }
    }
}
