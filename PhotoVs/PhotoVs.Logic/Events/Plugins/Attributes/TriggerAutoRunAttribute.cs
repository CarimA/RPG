using System;

namespace PhotoVs.Logic.Events.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TriggerAutoRunAttribute : Attribute
    {
    }
}
