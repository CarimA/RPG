using System;

namespace PhotoVs.Engine.ECS
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SystemAttribute : Attribute
    {
        public RunOn RunOn { get; }
        public Type[] RequiredTypes { get; }

        public SystemAttribute(RunOn runOn, params Type[] requiredTypes)
        {
            RunOn = runOn;
            RequiredTypes = requiredTypes;
        }
    }
}