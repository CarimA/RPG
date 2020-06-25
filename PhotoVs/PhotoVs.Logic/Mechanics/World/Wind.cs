﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PhotoVs.Logic.Mechanics.World
{
    public class Wind
    {
        public Vector2 Direction { get; private set; }
        public float Force { get; private set; }

        private Vector2 _targetDirection;
        private float _targetForce;

        private float _maxTimer; 
        private float _nextUpdate;
        private Random _random;

        private Vector2 _minRange;
        private Vector2 _maxRange;

        public Wind()
        {
            _random = new Random();
            _minRange = new Vector2(-1, -1);
            _maxRange = new Vector2(1, 1);
            _nextUpdate = 0;

            Direction = _random.NextVector2(_minRange, _maxRange);
            Force = (float)((_random.NextDouble() + 0.5f) * _random.Next(-4, 4));
        }


        public void Update(GameTime gameTime)
        {
            _nextUpdate -= gameTime.GetElapsedSeconds();

            if (_nextUpdate <= 0f)
            {
                _nextUpdate = (float) ((_random.NextDouble() + 0.5f) * _random.Next(2, 6));
                _maxTimer = _nextUpdate;
                _targetDirection = _random.NextVector2(_minRange, _maxRange);
                _targetForce = (float)((_random.NextDouble() + 0.5f) * _random.Next(-4, 4));
            }

            Direction = Vector2.SmoothStep(Direction, _targetDirection, (Progress()) / 10f);
            Force = MathHelper.SmoothStep(Force, _targetForce, (Progress()) / 10f);
        }

        public float Progress()
        {
            var half = _maxTimer / 2;
            if (_nextUpdate < half)
            {
                return (_nextUpdate / _maxTimer) / 2;
            }
            else
            {
                return 1 - ((_nextUpdate / _maxTimer) / 2);
            }
        }

        public float Speed()
        {
            return Math.Abs(Force) / 4f;
        }
    }
}