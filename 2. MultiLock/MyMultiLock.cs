using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/*
#task 2
Написать реализацию для интерфейса 

public interface IMultiLock
{
    public IDisposable AcquireLock(params string[] keys);
}

При вызове метода AcquireLock должна захватываться блокировка по набору ключей. Это значит, что если другой поток попытается
захватить блокировку (используя этот же метод) и в его набор ключей будет входить хотя бы один из ранее захваченных - то этот
другой поток заблокируется.
Метод должен возвращать IDisposable объект - при вызове Dispose() блокировка со всех ключей из набора должна сниматься.
Так же необходимо, чтобы любые два невложенных вызова AcquireLock не приводили к взаимоблокировке.
При необходимости вы можете принимать в конструкторе MultiLock-а набор ключей, по которым допустима блокировка.
 
*/
namespace MultiLock
{
    public class MyMultiLock : IMultiLock
    {
        public IDisposable AcquireLock(params string[] keys)
        {
            // Если строки имеют одинаковое значение, не факт, что они имеют одинаковый адрес в памяти.
            // Т.е нет гарантий, что при подстановке строк с одинаковым значением в object.ReferenceEquals(s1, s2)
            // вернётся true. string.Intern возвращает строку с одним и тем же адресом, при условии, что ему
            // передавать строки с одними и теми же значениями.
            var sortedKeys = keys.Distinct()
                .OrderBy(k => k)
                .Select(k => string.Intern(k))
                .ToArray();
            var lockTaken = false;
            try
            {
                foreach (var k in sortedKeys)
                {
                    lockTaken = false;
                    Monitor.Enter(k, ref lockTaken);
                }
                return new LockedKeys<string>(sortedKeys);
            }
            catch (Exception exception)
            {
                for (var i = sortedKeys.Length - 1; i >= 0; --i)
                {
                    if (Monitor.IsEntered(sortedKeys[i]))
                    {
                        Monitor.Exit(sortedKeys[i]);
                    }
                }
                throw;
            }
        }
    }
}

