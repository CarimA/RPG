using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Math;

namespace PhotoVs.Collision
{
    public struct CollisionResult
    {
        public bool WillIntersect;
        public bool AreIntersecting;
        public Vector2 MinimumTranslation;

        // todo: please refactor
        public static CollisionResult Simulate(Vector2 positionA,
            Vector2 positionB,
            RectangleF collisionBoundA,
            RectangleF collisionBoundB,
            List<Vector2> pointsA,
            List<Vector2> pointsB,
            List<Vector2> edgesA,
            List<Vector2> edgesB,
            Vector2 centerA,
            Vector2 centerB,
            Vector2 velocity)
        {
            var result = new CollisionResult
            {
                AreIntersecting = true,
                WillIntersect = true
            };

            var minimumInterval = float.PositiveInfinity;
            var translationAxis = Vector2.Zero;
            Vector2 edge;

            // loop through the edges of both polygons
            for (var edgeIndex = 0; edgeIndex < edgesA.Count + edgesB.Count; edgeIndex++)
            {
                edge = edgeIndex < edgesA.Count ? edgesA[edgeIndex] : edgesB[edgeIndex - edgesA.Count];

                // find if the polygons are intersecting
                // find the axis perpendicular to the current edge
                var axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // find the projection of the axis;
                var (minA, maxA) = ProjectPolygon(axis, positionA, pointsA);
                var (minB, maxB) = ProjectPolygon(axis, positionB, pointsB);

                // check if the projections are currently intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0)
                    result.AreIntersecting = false;

                // check if they will intersect
                var velocityProjection = Vector2.Dot(axis, velocity);

                if (velocityProjection < 0)
                    minA += velocityProjection;
                else
                    maxA += velocityProjection;

                var interval = IntervalDistance(minA, maxA, minB, maxB);
                if (interval > 0)
                    result.WillIntersect = false;

                if (!result.AreIntersecting && !result.WillIntersect)
                    break;

                interval = System.Math.Abs(interval);
                if (!(interval < minimumInterval))
                    continue;

                minimumInterval = interval;
                translationAxis = axis;

                var d = positionA + centerA - (positionB + centerB);
                if (Vector2.Dot(d, translationAxis) < 0)
                    translationAxis = -translationAxis;
            }

            if (result.WillIntersect)
                result.MinimumTranslation = translationAxis * minimumInterval;

            return result;
        }

        public static Vector2 GetFurthestFromOrigin(List<Vector2> velocities)
        {
            var x = 0f;
            var y = 0f;

            foreach (var point in velocities)
            {
                if (System.Math.Abs(point.X) > System.Math.Abs(x))
                    x = point.X;

                if (System.Math.Abs(point.Y) > System.Math.Abs(y))
                    y = point.Y;
            }

            return new Vector2(x, y);
        }

        private static (float, float) ProjectPolygon(Vector2 axis, Vector2 position, List<Vector2> points)
        {
            var d = Vector2.Dot(position + points[0], axis);
            var min = d;
            var max = d;

            foreach (var t in points)
            {
                d = Vector2.Dot(position + t, axis);
                if (d < min)
                    min = d;
                else if (d > max)
                    max = d;
            }

            return (min, max);
        }

        // calculate the distance between min/max, distance will be negative if intervals overlap
        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            return minA < minB ? minB - maxA : minA - maxB;
        }

        private static bool AreIntervalsIntersecting(float minA, float maxA, float minB, float maxB)
        {
            return !(IntervalDistance(minA, maxA, minB, maxB) > 0);
        }
    }
}