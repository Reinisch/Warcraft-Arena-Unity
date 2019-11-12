using System.Collections.Generic;

namespace Common
{
    public class ObjectPool<T> where T : class, new()
    {
        private readonly Stack<T> stack = new Stack<T>();

        public T Take() => stack.Count == 0 ? new T() : stack.Pop();

        public void Return(T element) => stack.Push(element);
    }
}