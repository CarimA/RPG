using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.World.Components
{
    public class CZone : IComponent
    {
        public string Name;

        public CZone(string name)
        {
            Name = name;
        }

        public bool Enabled { get; set; }
    }
}