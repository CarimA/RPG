using System.Collections;
using PhotoVs.Events;

namespace PhotoVs.Plugins
{
    public abstract class Plugin
    {
        public Plugin(GameEvents gameEvents)
        {
            GameEvents = gameEvents;
        }

        public abstract string Name { get; }
        public abstract string Version { get; }
        public GameEvents GameEvents { get; }

        public abstract void Bind(GameEvents gameEvents);

        public void Spawn(IEnumerator coroutine)
        {
        }

        public void Raise(string id, IGameEventArgs args = null)
        {
            GameEvents.Raise(id, args);
        }
    }
}