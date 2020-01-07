using PhotoVs.Engine.Scheduler.YieldInstructions;
using System;
using System.Collections;

namespace PhotoVs.Logic.Plugins
{
    public abstract partial class Plugin
    {
        public Dialogue Dialogue(string name, string dialogue)
        {
            return new Dialogue(Services.SceneMachine, name, dialogue);
        }

        public TextInput TextInput(string question, string defaultText = "", int limit = 15)
        {
            return new TextInput(Services.SceneMachine, question, defaultText, limit);
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
