using System;
using System.Collections.Generic;
using Zenject;

namespace Common
{
    public class GenericFactory<T> where T : class
    {
        [Inject]
        private DiContainer container;

        public T Create()
        {
            return container.Instantiate<T>();
        }
    }

    public abstract class GenericPoolItem : IDisposable
    {
        private GenericMemoryPool pool;

        internal bool IsTaken => pool != null;

        public void Dispose() => pool?.Return(this);

        internal void TakenFromPool(GenericMemoryPool pool)
        {
            this.pool = pool;
        }

        internal void ReturnToPool()
        {
            OnReturnedToPool();

            pool = null;
        }

        protected abstract void OnReturnedToPool();
    }

    public abstract class GenericPoolItem<TParam1> : GenericPoolItem
    {
        public void TakenFromPool(GenericMemoryPool pool, TParam1 param1)
        {
            base.TakenFromPool(pool);

            OnTakenFromPool(param1);
        }

        protected abstract void OnTakenFromPool(TParam1 param1);
    }

    public abstract class GenericPoolItem<TParam1, TParam2> : GenericPoolItem
    {
        public void TakenFromPool(GenericMemoryPool pool, TParam1 param1, TParam2 param2)
        {
            base.TakenFromPool(pool);

            OnTakenFromPool(param1, param2);
        }

        protected abstract void OnTakenFromPool(TParam1 param1, TParam2 param2);
    }

    public abstract class GenericMemoryPool
    {
        internal abstract void Return(object element);
    }

    public abstract class GenericMemoryPoolBase<T, TFactory> : GenericMemoryPool
        where T : GenericPoolItem
        where TFactory: GenericFactory<T>
    {
        [Inject]
        private TFactory factory;

        private readonly Stack<T> poolStack = new();

        [Inject]
        private void Populate([InjectOptional] int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
                poolStack.Push(factory.Create());
        }

        public void Return(T element)
        {
            Assert.IsTrue(element.IsTaken);

            if (element.IsTaken)
            {
                element.ReturnToPool();
                poolStack.Push(element);
            }
        }

        protected T TakeOrCreate() => poolStack.Count == 0 ? factory.Create() : poolStack.Pop();

        internal override void Return(object element) => Return((T)element);
    }

    public class GenericMemoryPool<T, TFactory> : GenericMemoryPoolBase<T, TFactory>
        where T : GenericPoolItem
        where TFactory : GenericFactory<T>
    {
        public T Take()
        {
            T element = TakeOrCreate();
            element.TakenFromPool(this);
            return element;
        }
    }

    public class GenericMemoryPool<T, TFactory, TParam1> :
        GenericMemoryPool<T, TFactory>
        where T : GenericPoolItem<TParam1>
        where TFactory : GenericFactory<T>
    {
        public T Take(TParam1 param1)
        {
            T element = Take();
            element.TakenFromPool(this, param1);
            return element;
        }
    }

    public class GenericMemoryPool<T, TFactory, TParam1, TParam2> :
        GenericMemoryPool<T, TFactory>
        where T : GenericPoolItem<TParam1, TParam2>
        where TFactory : GenericFactory<T>
    {
        public T Take(TParam1 param1, TParam2 param2)
        {
            T element = Take();
            element.TakenFromPool(this, param1, param2);
            return element;
        }
    }

}
