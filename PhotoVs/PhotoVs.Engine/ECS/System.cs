using System;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.ECS
{
    public class System : ISystem
    {
        private readonly SystemAttribute _system;

        public bool Enabled { get; set; }
        public Type[] RequiredComponents => _system.RequiredTypes;
        public int Priority { get; }
        public RunOn RunOn => _system.RunOn;

        public Action<GameTime, GameObjectList> Method { get; }

        public System(Action<GameTime, GameObjectList> method, int priority = 0)
        {
            Method = method;
            Priority = priority;
            _system = Method.GetMethodInfo().GetCustomAttribute<SystemAttribute>();
        }
    }
}