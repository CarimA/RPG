using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines.Instructions
{
    public class WaitFrame : IYield
    {
        public GameTime GameTime { get; private set; }

        public bool CanContinue(GameTime gameTime)
        {
            GameTime = gameTime;
            return true;
        }
    }
}