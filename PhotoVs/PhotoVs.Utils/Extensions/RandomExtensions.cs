using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Utils.Extensions
{
    public static class RandomExtensions
    {
        public static T NextShuffle<T>(this Random random, params T[] args)
        {
            return args[random.Next(args.Length)];
        }

        public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max)
        {
            return new Vector2(random.NextFloat(min.X, max.X), random.NextFloat(min.Y, max.Y));
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            return ((float)random.NextDouble() * (max - min)) + min;
        }

        public static Vector2 NextVector2(this Random random, Rectangle rectangle)
        {
            var x = random.NextFloat(rectangle.Left, rectangle.Right);
            var y = random.NextFloat(rectangle.Top, rectangle.Bottom);
            return new Vector2(x, y);
        }

        public static float ToAngle(this Vector2 direction)
        {
            return (float) Math.Atan2(direction.Y, direction.X);
        }

        public static Vector2 ToDirection(this float angle)
        {
            return new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
        }
    }
}