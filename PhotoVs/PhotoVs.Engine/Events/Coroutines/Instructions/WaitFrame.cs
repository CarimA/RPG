using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines.Instructions
{
    public class WaitFrame : IYield
    {
        public GameTime GameTime { get; private set; }

        public WaitFrame()
        {
        }

        public bool CanContinue(GameTime gameTime)
        {
            GameTime = gameTime;
            return true;
        }
    }
}
