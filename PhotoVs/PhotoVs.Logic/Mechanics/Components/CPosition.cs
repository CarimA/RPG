using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class CPosition
    {
        private Vector2 _position;

        public CPosition(Vector2 position)
        {
            _position = position;
            LastPosition = position;
            VelocityIntent = new List<Vector2>();
        }

        public Vector2 LastPosition { get; set; }

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
    }
}