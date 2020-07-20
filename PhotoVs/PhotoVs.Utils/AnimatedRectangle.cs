using System;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Utils
{
    public class AnimatedRectangle
    {
        private Rectangle _currentRectangle;
        private Rectangle _startRectangle;
        private Rectangle _targetRectangle;

        public Rectangle Current => _currentRectangle;

        private TimeSpan _updateTime;
        private float _progression;

        public float Progression => _progression;

        private Easings.Functions _easingFunction;

        public AnimatedRectangle(Rectangle rectangle, TimeSpan animatedTime, Easings.Functions easingFunction)
        {
            _currentRectangle = rectangle;
            _targetRectangle = rectangle;
            _startRectangle = rectangle;
            SetAnimatedTime(animatedTime);
            _easingFunction = easingFunction;
        }

        public void SetAnimatedTime(TimeSpan timeSpan)
        {
            _updateTime = timeSpan;
        }

        public void SetTargetRectangle(Rectangle rectangle)
        {
            _progression = 0;
            _startRectangle = _currentRectangle;
            _targetRectangle = rectangle;
        }

        public void Update(GameTime gameTime)
        {
            var increment = 1f / (float) _updateTime.TotalSeconds;
            _progression += increment * gameTime.GetElapsedSeconds();
            if (_progression > 1f)
                _progression = 1f;

            float cLeft = _startRectangle.Left;
            float cRight = _startRectangle.Right;
            float cTop = _startRectangle.Top;
            float cBottom = _startRectangle.Bottom;
            float tLeft = _targetRectangle.Left;
            float tRight = _targetRectangle.Right;
            float tTop = _targetRectangle.Top;
            float tBottom = _targetRectangle.Bottom;
            var ease = Easings.Interpolate(_progression, _easingFunction);

            var left = (int) ease.Map(0f, 1f, cLeft, tLeft);
            var top = (int) ease.Map(0f, 1f, cTop, tTop);

            _currentRectangle = new Rectangle(
                left,
                top,
                (int) ease.Map(0f, 1f, cRight, tRight) - left,
                (int) ease.Map(0f, 1f, cBottom, tBottom) - top
            );
        }
    }
}
