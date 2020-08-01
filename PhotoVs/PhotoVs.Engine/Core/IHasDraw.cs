using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IHasDraw
    {
        int DrawPriority { get; set; }
        bool DrawEnabled { get; set; }
        void Draw(GameTime gameTime);
    }
}