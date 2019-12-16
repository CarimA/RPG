using PhotoVs.ECS.Components;

namespace PhotoVs.WorldZoning
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