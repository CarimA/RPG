using PhotoVs.Models;
using System.Collections;

namespace PhotoVs.Engine.Plugins
{
    public abstract class Plugin
    {
        public Plugin(Events gameEvents)
        {
            GameEvents = gameEvents;
        }

        public abstract string Name { get; }
        public abstract string Version { get; }
        public Events GameEvents { get; }

        public abstract void Bind(Events gameEvents);

        public void Spawn(IEnumerator coroutine)
        {
        }
    }
}