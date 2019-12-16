using Microsoft.Xna.Framework;
using PhotoVs.ECS.Components;

namespace PhotoVs.CommonGameLogic.Transforms
{
    public class CPosition : IComponent
    {
        public Vector2 Position { get; set; }
    }
}