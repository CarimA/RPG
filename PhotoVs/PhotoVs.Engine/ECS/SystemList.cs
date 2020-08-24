using System;
using System.Collections;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS
{
    public class SystemList<T> : IList<T> where T : ISystem
    {
        private readonly List<T> _systems;

        public SystemList()
        {
            _systems = new List<T>();
        }

        public void Add(T system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            _systems.Add(system);
            _systems.Sort(SortByPriority);
        }

        public void Clear()
        {
            _systems.Clear();
        }

        public bool Contains(T item)
        {
            return _systems.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _systems.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _systems.Remove(item);
        }

        public int Count => _systems.Count;
        public bool IsReadOnly => false;

        private int SortByPriority(T a, T b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _systems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _systems.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _systems.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _systems.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _systems[index];
            set => _systems[index] = value;
        }
    }
}
