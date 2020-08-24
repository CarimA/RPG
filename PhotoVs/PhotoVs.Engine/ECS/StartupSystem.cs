using System;
using System.Reflection;

namespace PhotoVs.Engine.ECS
{
    public class StartupSystem : ISystem
    {
        private readonly SystemAttribute _system;

        public bool Enabled { get; set; }
        public Type[] RequiredComponents => _system.RequiredTypes;
        public int Priority { get; }
        public RunOn RunOn => _system.RunOn;

        public Action<GameObjectList> Method { get; }

        public StartupSystem(Action<GameObjectList> method, int priority = 0)
        {
            Method = method;
            Priority = priority;
            _system = Method.GetMethodInfo().GetCustomAttribute<SystemAttribute>();
        }
    }
}