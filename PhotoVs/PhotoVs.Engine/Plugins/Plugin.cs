using System.Collections;
using System.Collections.Generic;
using PhotoVs.Engine.Scheduler;

namespace PhotoVs.Engine.Plugins
{
    public abstract class Plugin
    {
        public abstract string Name { get; }
        public abstract string Version { get; }

        internal Coroutines _coroutines;

        public virtual void Bind(Events events)
        {

        }

        internal void BindCoroutines(Coroutines coroutines)
        {
            _coroutines = coroutines;
        }

        public void Spawn(IEnumerator routine)
        {
            _coroutines?.Start(routine);
        }
    }
}