using PhotoVs.Engine;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.YieldInstructions;
using System;
using System.Collections;

namespace PhotoVs.Logic
{
    public class Scheduler
    {
        private readonly Coroutines _coroutines;
        private readonly Player _player;
        private readonly SceneMachine _sceneMachine;

        public Scheduler(Services services)
        {
            _coroutines = services.Get<Coroutines>();
            _player = services.Get<Player>();
            _sceneMachine = services.Get<SceneMachine>();
        }

        public IEnumerator Spawn(IEnumerator routine)
        {
            _coroutines.Start(routine);
            return routine;
        }

        public IEnumerator LockMovement(Func<IEnumerator> action)
        {
            _player.LockMovement();
            yield return Spawn(action());
            _player.UnlockMovement();
        }

        public Dialogue Dialogue(string name, string dialogue)
        {
            return new Dialogue(_sceneMachine, name, dialogue);
        }

        public TextInput TextInput(string question, string defaultText = "", int limit = 15)
        {
            return new TextInput(_sceneMachine, question, defaultText, limit);
        }

        public Pause Pause(float time)
        {
            return new Pause(time);
        }
    }
}
