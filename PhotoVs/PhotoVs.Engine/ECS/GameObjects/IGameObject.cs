using System.Collections.Generic;

namespace PhotoVs.Engine.ECS
{
    public interface IGameObject
    {
        string Name { get; set; }
        IComponentCollection Components { get; }
        List<string> Tags { get; }
    }
}