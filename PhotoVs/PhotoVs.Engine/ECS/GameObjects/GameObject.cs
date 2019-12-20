using System.Collections.Generic;
using PhotoVs.Engine.ECS.Components;
using PhotoVs.Models.ECS;

namespace PhotoVs.Engine.ECS.GameObjects
{
    public class GameObject : IGameObject
    {
        public GameObject()
        {
            Components = new ComponentCollection();
            Tags = new List<string>();
        }

        public string Name { get; set; }
        public IComponentCollection Components { get; }
        public List<string> Tags { get; }
    }
}