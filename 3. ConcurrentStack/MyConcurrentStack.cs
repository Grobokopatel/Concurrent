using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentStack
{
    public class MyConcurrentStack<T> : IStack<T>
    {
        private class Node
        {
            internal readonly T _value;
            private Node _previous;
            internal Node Previous
            {
                get => _previous;

                set
                {
                    Index = value is null ? 0 : value.Index + 1;
                    _previous = value;
                }
            }

            internal int Index
            {
                get;
                private set;
            }

            internal Node(T value, Node previous = null)
            {
                _value = value;
                Previous = previous;
            }
        }
        private volatile Node _head;
        public MyConcurrentStack()
        {
        }

        public MyConcurrentStack(IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException();

            foreach (var e in collection)
            {
                var newNode = new Node(e, _head);
                _head = newNode;
            }
        }

        public int Count
        {
            get
            {
                var head = _head;
                return head is null ? 0 : head.Index + 1;
            }
        }
        

        public void Push(T item)
        {
            var spin = new SpinWait();
            var newNode = new Node(item, _head);
            while (Interlocked.CompareExchange(ref _head, newNode, newNode.Previous) != newNode.Previous)
            {
                spin.SpinOnce();
                newNode.Previous = _head;
            }
        }

        public bool TryPop(out T result)
        {
            var spin = new SpinWait();
            Node head;
            while (true)
            {
                head = _head;
                if (head == null)
                {
                    result = default(T);
                    return false;
                }

                if (Interlocked.CompareExchange(ref _head, head.Previous, head) == head)
                {
                    result = head._value;
                    return true;
                }

                spin.SpinOnce();
            }
        }
    }
}
