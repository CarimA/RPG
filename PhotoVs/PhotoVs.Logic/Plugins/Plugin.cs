using System;
using System.Collections;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Plugins
{
    public abstract class Plugin
    {
        public virtual string Name { get; }
        public virtual int Version { get; }

        internal Services Services { get; set; }

        public IEnumerator Spawn(IEnumerator routine)
        {
            Services.Get<Coroutines>().Start(routine);
            return routine;
        }

        public IEnumerator LockMovement(Func<IEnumerator> action)
        {
            var player = Services.Get<Player>();
            player.LockMovement();
            yield return Spawn(action());
            player.UnlockMovement();
        }

        public Dialogue Dialogue(string name, string dialogue)
        {
            return new Dialogue(Services.Get<SceneMachine>(), name, dialogue);
        }

        public TextInput TextInput(string question, string defaultText = "", int limit = 15)
        {
            return new TextInput(Services.Get<SceneMachine>(), question, defaultText, limit);
        }

        public Pause Pause(float time)
        {
            return new Pause(time);
        }
    }
}