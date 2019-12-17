using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.WorldZoning
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