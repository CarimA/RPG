using System;

namespace PhotoVs.Engine.Events.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TriggerAttribute : Attribute
    {
        public string RunOn { get; }

        public TriggerAttribute(string runOn)
        {
            RunOn = runOn;
        }
    }
}
