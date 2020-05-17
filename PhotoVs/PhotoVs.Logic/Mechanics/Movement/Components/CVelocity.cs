using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.Movement.Components
{
    public class CVelocity : IComponent
    {
        public Vector2 Velocity { get; set; }
        public List<Vector2> VelocityIntent { get; set; }

        public CVelocity()
        {
            VelocityIntent = new List<Vector2>();
        }
    }
}