using Microsoft.Xna.Framework;
using System;

namespace PhotoVs.Engine.Events.Coroutines.Instructions
{
    public class WaitUntil : IYield
    {
        private readonly Func<bool> _predicate;

        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public bool CanContinue(GameTime gameTime)
        {
            return !_predicate();
        }
    }
}
