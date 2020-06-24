using System;

namespace PhotoVs.Logic.Events.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TriggerAttribute : Attribute
    {
        public GameEvents GameEvent { get; }
        public string Delimiter { get; }

        public TriggerAttribute(GameEvents gameEvents)
        {
            GameEvent = gameEvents;
            Delimiter = string.Empty;
        }

        public TriggerAttribute(GameEvents gameEvents, string delimiter)
        {
            GameEvent = gameEvents;
            Delimiter = delimiter;
        }
    }
}
