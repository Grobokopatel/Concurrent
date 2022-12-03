
/*
#task 3
Написать свою реализацию класса lock-free ConcurrentStack. Для этого пригодятся методы класса Interlocked.


Стек должна реализовывать интерфейс


public interface IStack<T>
{
    void Push(T item);
    bool TryPop(out T item);
    int Count { get; }
}


Свойство Count должно работать за O(1)
На задачу отводится две недели и за неё можно получить два балла
Дедлайн 06.12 в 12:50 
*/
namespace ConcurrentStack
{
    public class Program
    {
        public static void Main()
        {
            
        }
    }
}