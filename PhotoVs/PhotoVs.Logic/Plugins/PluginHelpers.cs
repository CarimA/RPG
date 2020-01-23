using PhotoVs.Engine.Scheduler.YieldInstructions;
using System;
using System.Collections;

namespace PhotoVs.Logic.Plugins
{
    public abstract partial class Plugin
    {
        public Dialogue Dialogue(string name, string dialogue)
        {
            return new Dialogue(SceneMachine, name, dialogue);
        }

        public TextInput TextInput(string question, string defaultText = "", int limit = 15)
        {
            return new TextInput(SceneMachine, question, defaultText, limit);
        }

        public Pause Pause(float time)
        {
            return new Pause(time);
        }

        public IEnumerator LockMovement(Func<IEnumerator> action)
        {
            Player.LockMovement();
            yield return Spawn(action());
            Player.UnlockMovement();
        }
    }
}
