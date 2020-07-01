using Microsoft.Xna.Framework;
using System;

namespace PhotoVs.Utils.Extensions
{
    public static class RandomExtensions
    {
        public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max)
        {
            var x = (random.NextDouble() * (max.X - min.X) + (0 - max.X));
            var y = (random.NextDouble() * (max.Y - min.Y) + (0 - max.Y));

            return new Vector2((float)x, (float)y);
        }

        public static float ToAngle(this Vector2 direction)
        {
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public static Vector2 ToDirection(this float angle)
        {
            return new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
        }
    }
}
