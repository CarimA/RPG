using System;
using System.Collections;
using PhotoVs.Engine;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.Coroutines.Instructions;
using PhotoVs.Logic.Events.Instructions;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Events.Plugins
{
    public abstract class Plugin
    {
        public virtual string Name { get; }

        protected Services _services;
        public Services Services => _services;

        protected Plugin()
        {
        }

        public void Initialise(Services services)
        {
            _services = services;
        }

        public IEnumerator Spawn(IEnumerator routine)
        {
            var coroutineRunner = Services.Get<CoroutineRunner>();
            coroutineRunner.Start(new Coroutine(routine));
            return routine;
        }

        public Dialogue Dialogue(string name, string dialogue)
        {
            var sceneMachine = Services.Get<SceneMachine>();
            return new Dialogue(sceneMachine, name, dialogue);
        }

        public IEnumerator LockMovement(Func<IEnumerator> action)
        {
            var player = Services.Get<Player>();
            player.LockMovement();
            yield return Spawn(action());
            player.UnlockMovement();
        }

        public Wait Wait(float time)
        {
            return new Wait(time);
        }
    }
}
