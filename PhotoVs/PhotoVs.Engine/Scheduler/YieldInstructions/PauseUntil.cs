using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public class PauseUntil : IYieldInstruction
    {
        public Func<bool> Predicate;

        public PauseUntil(Func<bool> predicate)
        {
            Predicate = predicate;
        }

        public bool Continue(GameTime gameTime)
        {
            return !Predicate();
        }
    }
}