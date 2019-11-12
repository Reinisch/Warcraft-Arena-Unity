using System.Collections.Generic;

namespace Common
{
    public static class ListPoolContainer<T>
    {
        private static readonly ObjectPool<List<T>> Pool = new ObjectPool<List<T>>();

        public static List<T> Take() => Pool.Take();

        public static void Return(List<T> list)
        {
            list.Clear();

            Pool.Return(list);
        }
    }
}