using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Movement.Components;

namespace PhotoVs.Logic.Mechanics.Movement
{
    public struct CollisionResult
    {
        public bool WillIntersect;
        public bool AreIntersecting;
        public Vector2 MinimumTranslation;

        // todo: please refactor
        public static CollisionResult Simulate(GameObject moving, GameObject stationary, Vector2 velocity)
        {
            var positionA = moving.Components.Get<CPosition>();
            var positionB = stationary.Components.Get<CPosition>();
            var collisionBoundA = moving.Components.Get<CCollisionBound>();
            var collisionBoundB = stationary.Components.Get<CCollisionBound>();

            var result = new CollisionResult
            {
                AreIntersecting = true,
                WillIntersect = true
            };

            var minimumInterval = float.PositiveInfinity;
            var translationAxis = Vector2.Zero;
            Vector2 edge;

            var edgesA = collisionBoundA.Edges;
            var edgesB = collisionBoundB.Edges;

            // loop through the edges of both polygons
            for (var edgeIndex = 0; edgeIndex < edgesA.Count + edgesB.Count; edgeIndex++)
            {
                edge = edgeIndex < edgesA.Count ? edgesA[edgeIndex] : edgesB[edgeIndex - edgesA.Count];

                // find if the polygons are intersecting
                // find the axis perpendicular to the current edge
                var axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // find the projection of the axis;
                var (minA, maxA) = ProjectPolygon(axis, positionA, collisionBoundA);
                var (minB, maxB) = ProjectPolygon(axis, positionB, collisionBoundB);

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

                interval = Math.Abs(interval);
                if (!(interval < minimumInterval))
                    continue;

                minimumInterval = interval;
                translationAxis = axis;

                var d = positionA.Position + collisionBoundA.Center - (positionB.Position + collisionBoundB.Center);
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
                if (Math.Abs(point.X) > Math.Abs(x))
                    x = point.X;

                if (Math.Abs(point.Y) > Math.Abs(y))
                    y = point.Y;
            }

            return new Vector2(x, y);
        }

        private static (float, float) ProjectPolygon(Vector2 axis, CPosition position, CCollisionBound bounds)
        {
            var d = Vector2.Dot(position.Position + bounds.Points[0], axis);
            var min = d;
            var max = d;

            foreach (var t in bounds.Points)
            {
                d = Vector2.Dot(position.Position + t, axis);
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