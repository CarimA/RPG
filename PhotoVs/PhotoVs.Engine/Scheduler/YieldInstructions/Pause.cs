using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public class Pause : IYieldInstruction
    {
        public float Time;

        public Pause(float time)
        {
            Time = time;
        }

        public bool Continue(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Time -= dt;
            return Time <= 0f;
        }
    }
}
