using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IHasBeforeDraw
    {
        int BeforeDrawPriority { get; set; }
        bool BeforeDrawEnabled { get; set; }
        void BeforeDraw(GameTime gameTime);
    }
}