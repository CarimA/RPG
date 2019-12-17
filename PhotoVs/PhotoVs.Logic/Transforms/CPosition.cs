using Microsoft.Xna.Framework;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Transforms
{
    public class CPosition : IComponent
    {
        public Vector2 Position { get; set; }
    }
}