using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Utils.Collections
{
    public class Quadtree<T>
    {
        private readonly RectangleF _topLeftBoundary;
        private readonly RectangleF _topRightBoundary;
        private readonly RectangleF _bottomLeftBoundary;
        private readonly RectangleF _bottomRightBoundary;

        private Quadtree<T> _topLeft;
        private Quadtree<T> _topRight;
        private Quadtree<T> _bottomLeft;
        private Quadtree<T> _bottomRight;

        private readonly List<(T, RectangleF)> _items;

        public RectangleF Boundaries { get; }

        public Quadtree(RectangleF boundaries)
        {
            Boundaries = boundaries;
            _items = new List<(T, RectangleF)>();

            var halfWidth = boundaries.Width / 2;
            var halfHeight = boundaries.Height / 2;

            _topLeftBoundary = new RectangleF(boundaries.Left, boundaries.Top, halfWidth, halfHeight);
            _topRightBoundary = new RectangleF(boundaries.Left + halfWidth, boundaries.Top, halfWidth, halfHeight);
            _bottomLeftBoundary = new RectangleF(boundaries.Left, boundaries.Top + halfHeight, halfWidth, halfHeight);
            _bottomRightBoundary = new RectangleF(boundaries.Left + halfWidth, boundaries.Top + halfHeight, halfWidth, halfHeight);
        }

        public List<Quadtree<T>> GetChildren()
        {
            var output = new List<Quadtree<T>>();

            if (_topLeft != null)
                output.Add(_topLeft);

            if (_topRight != null)
                output.Add(_topRight);

            if (_bottomLeft != null)
                output.Add(_bottomLeft);

            if (_bottomRight != null)
                output.Add(_bottomRight);

            return output;
        }

        public int Count()
        {
            var count = _items.Count;

            foreach (var child in GetChildren())
                count += child.Count();

            return count;
        }

        public Quadtree<T> Add(T item, RectangleF boundary)
        {
            // first, check if it's completely within the bounds of a child node
            if (_topLeftBoundary.Contains(boundary))
            {
                _topLeft ??= new Quadtree<T>(_topLeftBoundary);
                return _topLeft.Add(item, boundary);
            }

            if (_topRightBoundary.Contains(boundary))
            {
                _topRight ??= new Quadtree<T>(_topRightBoundary);
                return _topRight.Add(item, boundary);
            }

            if (_bottomLeftBoundary.Contains(boundary))
            {
                _bottomLeft ??= new Quadtree<T>(_bottomLeftBoundary);
                return _bottomLeft.Add(item, boundary);
            }

            if (_bottomRightBoundary.Contains(boundary))
            {
                _bottomRight ??= new Quadtree<T>(_bottomRightBoundary);
                return _bottomRight.Add(item, boundary);
            }

            // if it isn't, add it to this one.
            _items.Add((item, boundary));
            return this;
        }

        public List<T> Find(Rectangle boundary)
        {
            return Find(new RectangleF(boundary.X, boundary.Y, boundary.Width, boundary.Height));
        }

        public List<T> Find(RectangleF boundary)
        {
            var stack = new Stack<Quadtree<T>>();
            var output = new List<T>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                // iterate through current items and check what intersects/is within it
                foreach (var (obj, bounds) in current._items)
                {
                    if (boundary.Contains(bounds)
                        || boundary.Intersects(bounds)
                        || bounds.Contains(boundary))
                        output.Add(obj);
                }

                foreach (var child in current.GetChildren())
                {
                    if (child.Boundaries.Contains(boundary)
                        || boundary.Intersects(child.Boundaries)
                        || boundary.Contains(child.Boundaries))
                    {
                        stack.Push(child);
                    }
                }
            }

            return output;
        }
    }
}
