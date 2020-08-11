using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Engine.ECS
{
    // todo: bind systems and system collection to kernel

    public class SystemList
    {
        private readonly List<System> _updateables;
        private readonly List<System> _drawables;

        public SystemList()
        {
            _updateables = new List<System>();
            _drawables = new List<System>();
        }

        public new void Add(Action<GameTime, GameObjectList> system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            var s = new System(system);
            switch (s.RunOn)
            { 
                case RunOn.Draw:
                    _drawables.Add(s);
                    _drawables.Sort(SortByPriority);
                    break;
                case RunOn.Update:
                    _updateables.Add(s);
                    _updateables.Sort(SortByPriority);
                    break;
                default:
                    throw new ArgumentException(nameof(s.RunOn));
            }
        }

        private int SortByPriority(System a, System b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public new void AddRange(IEnumerable<Action<GameTime, GameObjectList>> systems)
        {
            foreach (var system in systems)
                Add(system);
        }

        public IEnumerator<System> UpdateSystems => _updateables.GetEnumerator();
        public IEnumerator<System> DrawSystems => _drawables.GetEnumerator();

        [GameSystem(RunOn.Update, typeof(int), typeof(int))]
        public void DoThis(GameTime gameTime, GameObjectList gameObjects)
        {

        }
    }

    public class System
    {
        private readonly GameSystemAttribute _attribute;

        public Type[] Required => _attribute.Required;
        public int Priority => _attribute.Priority;
        public RunOn RunOn => _attribute.RunOn;

        public Action<GameTime, GameObjectList> Method { get; }

        public System(Action<GameTime, GameObjectList> method)
        {
            Method = method;

            var methodInfo = method.GetMethodInfo();
            _attribute =  methodInfo.GetCustomAttribute<GameSystemAttribute>();
        }
    }

    public enum RunOn
    {
        Update,
        Draw
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GameSystemAttribute : Attribute
    {

        public RunOn RunOn { get; }
        public int Priority { get; }
        public Type[] Required { get; }

        public GameSystemAttribute(RunOn runOn, params Type[] required) : this(runOn, 0, required)
        {

        }

        public GameSystemAttribute(RunOn runOn, int priority = 0, params Type[] required)
        {
            RunOn = runOn;
            Priority = priority;
            Required = required;
        }
    }
}