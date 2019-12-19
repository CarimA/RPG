using System;
using System.Collections;
using PhotoVs.Engine;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Logic.Services;

namespace PhotoVs.Logic.Plugins
{
    public abstract class Plugin
    {
        public abstract string Name { get; }
        public abstract string Version { get; }
        internal ServiceLocator Services { get; set; }

        public virtual void Bind(Events events)
        {

        }

        public IEnumerator Spawn(IEnumerator routine)
        {
            Services.Coroutines.Start(routine);
            return routine;
        }

        public Dialogue Dialogue(string name, string dialogue)
        {
            return new Dialogue(Services.SceneMachine, name, dialogue);
        }

        public Pause Pause(float time)
        {
            return new Pause(time);
        }

        public IEnumerator LockMovement(Func<IEnumerator> action)
        {
            Services.Player.LockMovement();

            yield return Spawn(action());

            Services.Player.UnlockMovement();
        }
    }
}