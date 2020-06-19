using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.Events.Coroutines.Instruction
{
    public class Wait : IYield
    {
        private float _time;

        public Wait(float time)
        {
            _time = time;
        }

        public bool CanContinue(GameTime gameTime)
        {
            var dt = gameTime.GetElapsedSeconds();
            _time -= dt;
            return _time <= 0f;
        }
    }
}
