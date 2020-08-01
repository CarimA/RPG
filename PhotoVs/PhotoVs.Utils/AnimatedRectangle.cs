using System;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Utils
{
    public class AnimatedRectangle
    {
        private readonly Easings.Functions _easingFunction;
        private Rectangle _startRectangle;
        private Rectangle _targetRectangle;

        private TimeSpan _updateTime;

        public AnimatedRectangle(Rectangle rectangle, TimeSpan animatedTime, Easings.Functions easingFunction)
        {
            Current = rectangle;
            _targetRectangle = rectangle;
            _startRectangle = rectangle;
            SetAnimatedTime(animatedTime);
            _easingFunction = easingFunction;
        }

        public Rectangle Current { get; private set; }

        public float Progression { get; private set; }

        public void SetAnimatedTime(TimeSpan timeSpan)
        {
            _updateTime = timeSpan;
        }

        public void SetTargetRectangle(Rectangle rectangle)
        {
            Progression = 0;
            _startRectangle = Current;
            _targetRectangle = rectangle;
        }

        public void Update(GameTime gameTime)
        {
            var increment = 1f / (float) _updateTime.TotalSeconds;
            Progression += increment * gameTime.GetElapsedSeconds();
            if (Progression > 1f)
                Progression = 1f;

            float cLeft = _startRectangle.Left;
            float cRight = _startRectangle.Right;
            float cTop = _startRectangle.Top;
            float cBottom = _startRectangle.Bottom;
            float tLeft = _targetRectangle.Left;
            float tRight = _targetRectangle.Right;
            float tTop = _targetRectangle.Top;
            float tBottom = _targetRectangle.Bottom;
            var ease = Easings.Interpolate(Progression, _easingFunction);

            var left = (int) ease.Map(0f, 1f, cLeft, tLeft);
            var top = (int) ease.Map(0f, 1f, cTop, tTop);

            Current = new Rectangle(
                left,
                top,
                (int) ease.Map(0f, 1f, cRight, tRight) - left,
                (int) ease.Map(0f, 1f, cBottom, tBottom) - top
            );
        }
    }
}