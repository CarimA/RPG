using PhotoVs.ECS.Components;

namespace PhotoVs.WorldZoning
{
    public class CZone : IComponent
    {
        public string Name;

        public CZone(string name)
        {
            Name = name;
        }
    }
}