using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines.Instruction
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