using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines.Instruction
{
    public interface IYield
    {
        bool CanContinue(GameTime gameTime);
    }
}
