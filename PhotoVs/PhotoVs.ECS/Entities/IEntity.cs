using PhotoVs.ECS.Components;
using PhotoVs.ECS.Entities;

namespace PhotoVs.Core.ECS.Entities
{
    public interface IEntity
    {
        string Name { get; set; }
        ComponentCollection Components { get; }
        TagList Tags { get; }
    }
}