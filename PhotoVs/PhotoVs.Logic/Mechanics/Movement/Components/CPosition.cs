﻿using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Components;
using System.Collections.Generic;

namespace PhotoVs.Logic.Mechanics.Movement.Components
{
    public class CPosition : IComponent
    {
        public Vector2 LastPosition { get; set; }
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                LastPosition = _position;
                _position = value;
            }
        }
        public List<Vector2> VelocityIntent { get; set; }

        public Vector2 DeltaPosition => Position - LastPosition;

        public CPosition()
        {
            VelocityIntent = new List<Vector2>();
        }
    }
}