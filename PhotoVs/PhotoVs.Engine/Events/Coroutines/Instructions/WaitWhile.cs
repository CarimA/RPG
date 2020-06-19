using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines.Instructions
{
    public class WaitWhile : IYield
    {
        private readonly Func<bool> _predicate;

        public WaitWhile(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public bool CanContinue(GameTime gameTime)
        {
            return _predicate();
        }
    }
}