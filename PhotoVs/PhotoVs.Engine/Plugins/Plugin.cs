using PhotoVs.Engine.Scheduler;
using System.Collections;

namespace PhotoVs.Engine.Plugins
{
    public abstract class Plugin
    {
        public abstract string Name { get; }
        public abstract string Version { get; }
        internal Coroutines Coroutines { get; set; }

        public virtual void Bind(Events events)
        {

        }

        public void Spawn(IEnumerator routine)
        {
            Coroutines?.Start(routine);
        }
    }
}