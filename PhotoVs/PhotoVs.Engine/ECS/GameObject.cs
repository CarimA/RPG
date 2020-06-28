using System;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS
{
    public class GameObject
    {
        public GameObject() : this(string.Empty)
        {
        }

        public GameObject(string name)
        {
            ID = Guid.NewGuid().ToString();
            Name = name;
            Components = new ComponentList();
            Tags = new List<string>();
            Enabled = true;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public ComponentList Components { get; }
        public List<string> Tags { get; }
        public bool Enabled { get; set; }
    }
}