using System;

namespace PhotoVs.Logic.Events.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RunOnceAttribute : Attribute
    {
        public string OptionalFlag { get; }

        public RunOnceAttribute(string optionalFlag = default)
        {
            OptionalFlag = default;
        }
    }
}
