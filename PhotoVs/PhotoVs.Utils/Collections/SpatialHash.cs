using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Utils.Collections
{
    public class SpatialHash<TItem> : SpatialHash<TItem, List<TItem>> where TItem : new()
    {
        public SpatialHash(int cellSize) : base(cellSize)
        {
            
        }
    }

    public class SpatialHash<TItem, TList> where TList : IList<TItem>, new()
    {
        private readonly Dictionary<int, TList> _cells;
        private readonly int _cellSize;
        private readonly TList _empty;

        public SpatialHash(int cellSize)
        {
            _cells = new Dictionary<int, TList>();
            _empty = new TList();
            _cellSize = cellSize;
        }

        public void AddPoint(TItem item, int x, int y)
        {
            var tX = Snap(x);
            var tY = Snap(y);
            var key = HashPosition(tX, tY);

            if (!_cells.ContainsKey(key))
                _cells[key] = new TList();

            _cells[key].Add(item);
        }

        public void Add(TItem item, Rectangle bounds)
        {
            ForRange(bounds, (x, y) => { AddPoint(item, x, y); });
        }

        public void Add(TItem item, RectangleF bounds)
        {
            ForRange(bounds, (x, y) => { AddPoint(item, x, y); });
        }

        public TList GetPoint(int x, int y)
        {
            return _cells.TryGetValue(HashPosition(x, y), out var value) ? value : _empty;
        }

        public TList Get(Rectangle bounds)
        {
            var output = new TList();

            var snapLeft = Snap(bounds.Left) - _cellSize;
            var snapRight = Snap(bounds.Right) + _cellSize;
            var snapTop = Snap(bounds.Top) - _cellSize;
            var snapBottom = Snap(bounds.Bottom) + _cellSize;

            for (var x = snapLeft; x <= snapRight; x += _cellSize)
                for (var y = snapTop; y <= snapBottom; y += _cellSize)
                {
                    var points = GetPoint(x, y);
                    foreach (var point in points)
                        //if (!output.Contains(point))
                        output.Add(point);
                }

            return output;
        }

        private void ForRange(RectangleF range, Action<int, int> action)
        {
            for (var x = Snap(range.Left); x <= Snap(range.Right); x += _cellSize)
                for (var y = Snap(range.Top); y <= Snap(range.Bottom); y += _cellSize)
                    action(x, y);
        }

        private void ForRange(Rectangle range, Action<int, int> action)
        {
            var snapLeft = Snap(range.Left) - _cellSize;
            var snapRight = Snap(range.Right) + _cellSize;
            var snapTop = Snap(range.Top) - _cellSize;
            var snapBottom = Snap(range.Bottom) + _cellSize;

            for (var x = snapLeft; x <= snapRight; x += _cellSize)
                for (var y = snapTop; y <= snapBottom; y += _cellSize)
                    action(x, y);
        }

        private int Snap(float input)
        {
            return (int)(Math.Round(input / _cellSize) * _cellSize);
        }

        private static int HashPosition(int x, int y)
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                return hash * (0x27d4eb2d + HashInt(x)) * 0x27d4eb2d + HashInt(y);
            }
        }

        private static int HashInt(int val)
        {
            unchecked
            {
                const int c2 = 0x27d4eb2d;
                val = val ^ 61 ^ (val >> 16);
                val += val << 3;
                val ^= val >> 4;
                val *= c2;
                val ^= val >> 15;
                return val;
            }
        }
    }
}