using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Transforms
{
    public class CVelocity : IComponent
    {
        public CVelocity()
        {
            VelocityIntent = new List<Vector2>();
        }

        public Vector2 Velocity { get; set; }
        public List<Vector2> VelocityIntent { get; set; }
    }
}