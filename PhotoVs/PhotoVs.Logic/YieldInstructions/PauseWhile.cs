using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public class PauseWhile : IYieldInstruction
    {
        public Func<bool> Predicate;

        public PauseWhile(Func<bool> predicate)
        {
            Predicate = predicate;
        }

        public bool Continue(GameTime gameTime)
        {
            return Predicate();
        }
    }
}