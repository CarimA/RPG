using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.World.Components
{
    public class CScript : IComponent
    {
        public string Name;

        public CScript(string name)
        {
            Name = name;
        }
    }
}