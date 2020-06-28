using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PhotoVs.Logic.Mechanics.Movement.Components
{
    public class CPosition
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

        public CPosition(Vector2 position)
        {
            _position = position;
            LastPosition = position;
            VelocityIntent = new List<Vector2>();
        }
    }
}