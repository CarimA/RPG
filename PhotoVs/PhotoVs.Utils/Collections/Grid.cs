using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhotoVs.Utils.Collections
{
    public class Grid<T>
    {
        private readonly Dictionary<int, T> _cells;

        public Grid()
        {
            _cells = new Dictionary<int, T>();
        }

        public T this[int x, int y]
        {
            get => Get(x, y);
            set => Set(x, y, value);
        }

        public void Remove(int x, int y)
        {
            if (Get(x, y) != null)
                _cells.Remove(HashPosition(x, y));
        }

        public void Insert(int x, int y, T value)
        {
            if (Get(x, y) != null)
                throw new InvalidOperationException("Data already exists at " + x + "," + y + ". May be a collision?");

            Set(x, y, value);
        }

        public void Set(int x, int y, T value)
        {
            _cells[HashPosition(x, y)] = value;
        }

        public void Insert(Rectangle bounds, T value)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
            for (var y = bounds.Top; y < bounds.Bottom; y++)
                Insert(x, y, value);
        }

        public T Get(int x, int y)
        {
            return _cells.TryGetValue(HashPosition(x, y), out var value) ? value : default;
        }

        public IEnumerable<T> Get(Rectangle bounds)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
            for (var y = bounds.Top; y < bounds.Bottom; y++)
            {
                var cell = Get(x, y);
                if (cell != null)
                    yield return cell;
            }
        }

        private static int HashPosition(int x, int y)
        {
            unchecked
            {
                // these magic numbers are all prime numbers
                var hash = 463003;
                hash = hash * 997651 + x.GetHashCode();
                hash = hash * 148091 + y.GetHashCode();
                return hash * (0x27d4eb2d + HashInt(x)) * 0x27d4eb2d + HashInt(y);
            }
        }

        private static int HashInt(int val)
        {
            unchecked
            {
                return val * 0x27d4eb2d >> 24;
            }
        }
    }
}