using PhotoVs.Engine.ECS.Components;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS.GameObjects
{
    public interface IGameObject
    {
        string ID { get; set; }
        string Name { get; set; }
        IComponentCollection Components { get; }
        List<string> Tags { get; }
    }
}