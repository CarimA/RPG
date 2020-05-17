using Microsoft.Xna.Framework;
using System;

namespace PhotoVs.Utils.Extensions
{
    public static class GameTimeExtensions
    {
        public static float GetElapsedSeconds(this GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException(nameof(gameTime));

            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}