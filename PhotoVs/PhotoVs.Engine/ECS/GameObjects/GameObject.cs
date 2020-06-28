using System;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS.GameObjects
{
    public class GameObject : IGameObject
    {
        public GameObject()
        {
            ID = Guid.NewGuid().ToString();
            Components = new ComponentList();
            Tags = new List<string>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public ComponentList Components { get; }
        public List<string> Tags { get; }
    }
}