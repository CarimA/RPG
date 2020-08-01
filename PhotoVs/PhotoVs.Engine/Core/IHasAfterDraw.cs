using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IHasAfterDraw
    {
        int AfterDrawPriority { get; set; }
        bool AfterDrawEnabled { get; set; }
        void AfterDraw(GameTime gameTime);
    }
}