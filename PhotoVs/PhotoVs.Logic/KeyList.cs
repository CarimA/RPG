﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace PhotoVs.Logic
{
    internal sealed class KeyList<T> : List<T>, IEquatable<KeyList<T>>, IEnumerable
    {
        private int _hashCode;
        private readonly List<T> _items;

        public KeyList()
        {
            _items = new List<T>();
            _hashCode = 0;
        }

        public KeyList(int capacity)
        {
            _items = new List<T>(capacity);
            _hashCode = 0;
        }

        public T this[int index] => _items[index];

        public int Count => _items.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(KeyList<T> rhs)
        {
            var ic = _items.Count;
            if (ic != rhs._items.Count)
                return false;
            for (var i = 0; i < ic; ++i)
                if (!Equals(_items[i], rhs._items[i]))
                    return false;
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(T item)
        {
            _items.Add(item);
            if (null != item)
                _hashCode ^= item.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyList<T>);
        }
    }
}