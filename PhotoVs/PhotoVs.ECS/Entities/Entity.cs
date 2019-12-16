using PhotoVs.ECS.Components;
using PhotoVs.ECS.Entities;

namespace PhotoVs.Core.ECS.Entities
{
    public class Entity : IEntity
    {
        public Entity()
        {
            Components = new ComponentCollection();
            Tags = new TagList();
        }

        public string Name { get; set; }
        public ComponentCollection Components { get; }
        public TagList Tags { get; }
    }
}