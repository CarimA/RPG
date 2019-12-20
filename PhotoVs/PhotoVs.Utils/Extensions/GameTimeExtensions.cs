using Microsoft.Xna.Framework;

namespace PhotoVs.Utils.Extensions
{
    public static class GameTimeExtensions
    {
        public static float GetElapsedSeconds(this GameTime gameTime)
        {
            return (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}