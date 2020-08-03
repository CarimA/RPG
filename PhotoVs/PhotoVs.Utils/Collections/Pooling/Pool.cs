using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Utils.Collections.Pooling
{
    public class Pool<T> : IPool<T>
    {
        private readonly T[] _instances;
        private int _nextAvailable;
        private int _nextReturn;

        public Action<T> Reset { get; }
        public Action<T> New { get; }

        public int Free { get; private set; }
        public int Total { get; }

        public Pool(int total)
        {
            _instances = new T[total];
            for (var i = 0; i < total; i++)
                _instances[i] = default;

            _nextAvailable = 0;
            _nextReturn = 0;

            Total = total;
            Free = total;
        }

        public T Get()
        {
            if (_nextAvailable == -1)
                throw new OutOfMemoryException("Pool has exceeded number of available instances.");

            Free--;

            var instance = _instances[_nextAvailable];

            if (_nextReturn == -1)
                _nextReturn = _nextAvailable;

            _nextAvailable++;
            if (_nextAvailable == _nextReturn)
                _nextAvailable = -1;
            else if (_nextAvailable >= Total)
            {
                if (_nextReturn > 0)
                    _nextAvailable = 0;
                else
                    _nextAvailable = -1;
            }

            New?.Invoke(instance);
            return instance;
        }

        public void Release(T instance)
        {
            if (_nextReturn == -1)
                throw new OutOfMemoryException("Pool can not release any more objects.");

            Free++;

            Reset?.Invoke(instance);

            _instances[_nextReturn] = instance;

            if (_nextAvailable == -1)
                _nextAvailable = _nextReturn;

            _nextReturn++;
            if (_nextReturn >= Total)
                _nextReturn = 0;

            if (_nextReturn == _nextAvailable)
                _nextReturn = -1;
        }

        public void ReleaseAll()
        {
            _nextAvailable = 0;
            _nextReturn = -1;
            Free = Total;

            foreach (var i in _instances)
                Reset?.Invoke(i);
        }
    }
}
