using System.Collections;
using PhotoVs.Engine;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Plugins
{
    public abstract partial class Plugin
    {
        public abstract string Name { get; }
        public abstract string Version { get; }
        internal Coroutines Coroutines { get; set; }
        internal SceneMachine SceneMachine { get; set; }
        internal Player Player { get; set; }

        public virtual void Bind(Services services)
        {
        }

        public IEnumerator Spawn(IEnumerator routine)
        {
            Coroutines.Start(routine);
            return routine;
        }
    }
}