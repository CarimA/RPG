using System.Collections;
using PhotoVs.Engine;

namespace PhotoVs.Logic.Plugins
{
    public abstract partial class Plugin
    {
        public abstract string Name { get; }
        public abstract string Version { get; }
        internal Services Services { get; set; }

        public virtual void Bind(Events events)
        {
        }

        public IEnumerator Spawn(IEnumerator routine)
        {
            Services.Coroutines.Start(routine);
            return routine;
        }
    }
}