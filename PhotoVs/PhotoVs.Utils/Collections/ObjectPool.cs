using System;
using System.Collections.Generic;

namespace PhotoVs.Utils.Collections
{
    public class ObjectPool<T> where T : new()
    {
        private readonly HashSet<T> _allocated;
        private readonly Queue<T> _free;
        private readonly int _max;

        public ObjectPool(int max)
        {
            _free = new Queue<T>(max);
            _allocated = new HashSet<T>();
            _max = max;

            for (var i = 0; i < max; i++)
                _free.Enqueue(Spawn());
        }

        private static T Spawn()
        {
            return new T();
        }

        public T Free()
        {
            // retrieve a free object, otherwise create a new one if none available
            var item = _free.Count > 0
                ? _free.Dequeue()
                : Spawn();
            _allocated.Add(item);
            return item;
        }

        public bool Release(T item)
        {
            if (_allocated.Remove(item))
                if (_free.Count < _max)
                {
                    _free.Enqueue(item);
                    return true;
                }

            // if it's not in the allocated set or there's no space, just drop it
            // eventually the garbage collector will pick it up
            return false;
        }

        private void ReleaseAction(T item)
        {
            Release(item);
        }

        public void ReleaseAll()
        {
            ForEach(ReleaseAction);
        }

        public void ForEach(Action<T> action)
        {
            foreach (var item in _allocated)
                action(item);
        }

        public IEnumerable<T> GetAllocated()
        {
            return _allocated;
        }

        public int FreeCount()
        {
            return _free.Count;
        }
    }
}