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

        public static Direction ToDirection(this Vector2 direction)
        {
            var angle = direction.ToAngle() + MathHelper.PiOver2;
            var degrees = angle * (180 / MathHelper.Pi);

            var isUp = degrees >= -46 && degrees <= 46;
            
            var down = (degrees - 180);
            var isDown = down >= -46 && down <= 46;

            var left = (degrees - 270);
            var isLeft = left >= -46 && left <= 46;

            var right = (degrees - 90);
            var isRight = right >= -46 && right <= 46;

            if (isUp)
                return Direction.Up;
            else if (isDown)
                return Direction.Down;
            else if (isLeft)
                return Direction.Left;
            else if (isRight)
                return Direction.Right;

            return Direction.Down;
        }

        public static Vector2 ToDirection(this float angle)
        {
            return new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
        }
    }

    public enum Direction
    {
        Up, Down, Left, Right
    }
}