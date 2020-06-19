using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.Coroutines.Instruction;
using PhotoVs.Logic.Events.Instructions;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Events
{
    public class EventCommands
    {
        private readonly Player _player;
        private readonly CoroutineRunner _coroutineRunner;
        private SceneMachine _sceneMachine;

        public EventCommands(Services services)
        {
            _player = services.Get<Player>();
            _coroutineRunner = services.Get<CoroutineRunner>();
            _sceneMachine = services.Get<SceneMachine>();
        }

        public IEnumerator Spawn(IEnumerator routine)
        {
            _coroutineRunner.Start(new Coroutine(routine));
            return routine;
        }

        public Dialogue Dialogue(string name, string dialogue)
        {
            return new Dialogue(_sceneMachine, name, dialogue);
        }

        public IEnumerator LockMovement(Func<IEnumerator> action)
        {
            _player.LockMovement();
            yield return Spawn(action());
            _player.UnlockMovement();
        }

        public Wait Wait(float time)
        {
            return new Wait(time);
        }
    }
}
