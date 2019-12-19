using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public interface IYieldInstruction
    {
        bool Continue(GameTime gameTime);
    }
}
