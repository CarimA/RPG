using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.Movement.Components
{
    public class CPosition : IComponent
    {
        public Vector2 Position { get; set; }
    }
}