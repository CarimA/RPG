using System;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.World
{
    public class Wind
    {
        private readonly Vector2 _maxRange;

        private float _maxTimer;

        private readonly Vector2 _minRange;
        private float _nextUpdate;
        private readonly Random _random;

        private Vector2 _targetDirection;
        private float _targetForce;

        public Wind()
        {
            _random = new Random();
            _minRange = new Vector2(-1, -1);
            _maxRange = new Vector2(1, 1);
            _nextUpdate = 0;

            Direction = _random.NextVector2(_minRange, _maxRange);
            Force = (float) ((_random.NextDouble() + 0.5f) * _random.Next(-4, 4));
        }

        public Vector2 Direction { get; private set; }
        public float Force { get; private set; }


        public void Update(GameTime gameTime)
        {
            _nextUpdate -= gameTime.GetElapsedSeconds();

            if (_nextUpdate <= 0f)
            {
                _nextUpdate = (float) ((_random.NextDouble() + 0.5f) * _random.Next(2, 6));
                _maxTimer = _nextUpdate;
                _targetDirection = _random.NextVector2(_minRange, _maxRange);
                _targetForce = (float) _random.NextDouble() * 0.5f;
            }

            var currentAngle = Direction.ToAngle();
            var newAngle = _targetDirection.ToAngle();
            var step = MathHelper.Lerp(currentAngle, newAngle, 0.15f * gameTime.GetElapsedSeconds());
            Direction = step.ToDirection();
            Force = MathHelper.SmoothStep(Force, _targetForce, Progress() / 10f);
        }

        public float Progress()
        {
            var x = _nextUpdate / _maxTimer;
            var xn = x - 0.5f;
            var xs = xn * xn;
            return -(xs * 4f) + 1f;
        }
    }
}