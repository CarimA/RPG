using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public interface IYieldInstruction
    {
        bool Continue(GameTime gameTime);
    }
}
