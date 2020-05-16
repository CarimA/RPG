using PhotoVs.Engine.ECS;

namespace PhotoVs.Logic.WorldZoning
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