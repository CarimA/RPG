using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Utils.Collections
{
    public class Grid<T>
    {
        private readonly Dictionary<long, T> _cells;

        public T this[int x, int y]
        {
            get =>
                _cells.TryGetValue(HashPosition(x, y), out var value)
                    ? value
                    : default;
            set => _cells[HashPosition(x, y)] = value;
        }

        public Grid()
        {
            _cells = new Dictionary<long, T>();
        }

        public void Remove(int x, int y)
        {
            if (this[x, y] != null)
                _cells.Remove(HashPosition(x, y));
        }

        public void Insert(int x, int y, T value)
        {
            if (this[x, y] != null)
                throw new InvalidOperationException("Data already exists at " + x + "," + y + ". May be a collision?");

            this[x, y] = value;
        }

        public void Insert(Rectangle bounds, T value)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
                for (var y = bounds.Top; y < bounds.Bottom; y++)
                    Insert(x, y, value);
        }

        public IEnumerable<T> GetInBoundary(Rectangle bounds)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
                for (var y = bounds.Top; y < bounds.Bottom; y++)
                {
                    var cell = this[x, y];
                    if (cell != null)
                        yield return cell;
                }
        }

        private static long HashPosition(int x, int y)
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

        private static long HashInt(int val)
        {
            unchecked
            {
                return (val * 0x9e3779b1) >> 24;
            }
        }

        public List<T> ToList()
        {
            return _cells.Values.ToList();
        }
    }
}