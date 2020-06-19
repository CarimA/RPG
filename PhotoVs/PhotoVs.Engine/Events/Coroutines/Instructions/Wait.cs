using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.Events.Coroutines.Instructions
{
    public class Wait : IYield
    {
        private float _time;

        public Wait(float time)
        {
            _time = time;
        }

        public void SetTime(float time)
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
