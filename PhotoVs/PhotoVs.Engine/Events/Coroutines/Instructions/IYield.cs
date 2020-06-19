using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines.Instructions
{
    public interface IYield
    {
        bool CanContinue(GameTime gameTime);
    }
}
