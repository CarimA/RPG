using System;
using PhotoVs.Engine.ECS.Components;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS.GameObjects
{
    public class GameObject : IGameObject
    {
        public GameObject()
        {
            ID = Guid.NewGuid().ToString();
            Components = new ComponentCollection();
            Tags = new List<string>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public IComponentCollection Components { get; }
        public List<string> Tags { get; }
    }
}