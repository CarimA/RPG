using System.Collections.Generic;
using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Engine.ECS.GameObjects
{
    public interface IGameObject
    {
        string Name { get; set; }
        IComponentCollection Components { get; }
        List<string> Tags { get; }
    }
}