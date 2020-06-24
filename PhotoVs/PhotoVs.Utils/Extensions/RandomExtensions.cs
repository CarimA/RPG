using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Utils.Extensions
{
    public static class RandomExtensions
    {
        public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max)
        {
            var x = (random.NextDouble() * (max.X - min.X) + (0 - min.X));
            var y = (random.NextDouble() * (max.Y - min.Y) + (0 - min.Y));

            return new Vector2((float)x, (float)y);
        }
    }
}
