using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Utils;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class CCollisionBound
    {
        private const int Inflate = 10;

        public RectangleF Bounds;

        public Vector2 Center;
        public List<Vector2> Edges;
        public RectangleF InflatedBounds;

        // public Vector2 Origin;
        public List<Vector2> Points;

        public CCollisionBound(List<Vector2> points)
        {
            Points = ShiftPoints(points);
            Edges = BuildEdges(Points);
            var (minX, minY, maxX, maxY) = FindBounds(Points);
            Bounds = new RectangleF(0, 0, maxX - minX, maxY - minY);
            InflatedBounds = new RectangleF(Bounds.X - Inflate, Bounds.Y - Inflate,
                Bounds.Width + Inflate * 2, Bounds.Height + Inflate * 2);
            Center = FindCenter(Points);
        }

        private static List<Vector2> ShiftPoints(List<Vector2> points)
        {
            var x = 0f;
            var y = 0f;
            foreach (var point in points)
            {
                if (point.X < x)
                    x = point.X;

                if (point.Y < y)
                    y = point.Y;
            }

            return points.Select(point => new Vector2(point.X - x, point.Y - y)).ToList();
        }

        private static List<Vector2> BuildEdges(List<Vector2> points)
        {
            var edges = new List<Vector2>();
            Vector2 p1, p2;

            for (var i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                p2 = i + 1 >= points.Count ? points[0] : points[i + 1];
                edges.Add(p2 - p1);
            }

            return edges;
        }

        private static (float, float, float, float) FindBounds(List<Vector2> points)
        {
            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;

            foreach (var (x, y) in points)
            {
                if (x < minX)
                    minX = x;

                if (y < minY)
                    minY = y;

                if (x > maxX)
                    maxX = x;

                if (y > maxY)
                    maxY = y;
            }

            return (minX, minY, maxX - minX, maxY - minY);
        }

        private static Vector2 FindCenter(List<Vector2> points)
        {
            float tX = 0, tY = 0;
            foreach (var (x, y) in points)
            {
                tX += x;
                tY += y;
            }

            return new Vector2(tX / points.Count, tY / points.Count);
        }

        public static CCollisionBound Circle(float radius, int sides = 16)
        {
            var points = new List<Vector2>();
            var angle = MathHelper.Pi * 2 / sides;

            for (var i = 0; i < sides; i++)
                points.Add(
                    new Vector2(
                        radius * (float) Math.Cos(angle * i) + radius,
                        radius * (float) Math.Sin(angle * i) + radius));

            return new CCollisionBound(points);
        }

        public static CCollisionBound Rectangle(Size2 size)
        {
            var points = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(size.Width, 0),
                new Vector2(size.Width, size.Height),
                new Vector2(0, size.Height)
            };
            return new CCollisionBound(points);
        }

        public static CCollisionBound RectangularOctogon(Size2 size, float bevelLength)
        {
            var points = new List<Vector2>
            {
                new Vector2(bevelLength, 0),
                new Vector2(size.Width - bevelLength, 0),
                new Vector2(size.Width, bevelLength),
                new Vector2(size.Width, size.Height - bevelLength),
                new Vector2(size.Width - bevelLength, size.Height),
                new Vector2(bevelLength, size.Height),
                new Vector2(0, size.Height - bevelLength),
                new Vector2(0, bevelLength)
            };
            return new CCollisionBound(points);
        }
    }
}