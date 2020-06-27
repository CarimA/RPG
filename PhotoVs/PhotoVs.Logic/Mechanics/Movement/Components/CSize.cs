using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.Movement.Components
{
    public class CSize : IComponent
    {
        public Vector2 Size;
        public bool Enabled { get; set; }
    }
}