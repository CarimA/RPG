using System;
using System.Collections;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.Events;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.Coroutines.Instructions;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Events.Instructions;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using PhotoVs.Utils.Extensions;

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

        public IEnumerator Move(IGameObject gameObject, Vector2 targetPosition, float speed)
        {
            var position = gameObject.Components.Get<CPosition>();
            var done = false;
            WaitFrame waitFrame;

            while (!done)
            {
                waitFrame = new WaitFrame();
                yield return waitFrame;

                var direction = targetPosition - position.Position;
                direction.Normalize();

                var dt = waitFrame.GameTime.GetElapsedSeconds();
                var amount = speed * dt;

                if (Vector2.Distance(position.Position, targetPosition) < amount)
                {
                    position.Position = targetPosition;
                    done = true;
                }
                else
                {
                    position.Position += direction * amount;
                }
            }
        }

        public Wait Wait(float time)
        {
            return new Wait(time);
        }

        public string GetText(string text)
        {
            var textDb = Services.Get<TextDatabase>();
            return textDb.GetText(text);
        }

        public int PickChoice(string[] choices)
        {
            throw new NotImplementedException();
        }

        public bool PlayerFlag(string flag, bool? value = null)
        {
            var player = Services.Get<Player>();
            if (value.HasValue)
            {
                // set to this value
                var v = value.Value;
                player.PlayerData.SetFlag(flag, v);
                return v;
            }
            else
            {
                // get the value
                return player.PlayerData.GetFlag(flag);
            }
        }

        public IComparable PlayerVariable(string variable, IComparable value = null)
        {
            var player = Services.Get<Player>();
            if (value != null)
            {
                // set to this value
                player.PlayerData.SetVariable(variable, value);
                return value;
            }
            else
            {
                // get the value
                return player.PlayerData.GetVariable(variable);
            }
        }

        public void Save()
        {
            var player = Services.Get<Player>();
            player.PlayerData.Save();
        }

        public void Notify(string eventType, IGameEventArgs gameEvent)
        {
            var eventQueue = Services.Get<EventQueue>();
            eventQueue.Notify(eventType, gameEvent);
        }

        public void Warp(string map, Vector2 position)
        {
            var player = Services.Get<Player>();
            player.PlayerData.CurrentMap = map;
            player.PlayerData.Position.Position = position;
        }

        public void Warp(Vector2 position)
        {
            var player = Services.Get<Player>();
            player.PlayerData.Position.Position = position;
        }

        public IGameObject GetGameObjectByName(string name)
        {
            var sceneManager = Services.Get<IGameObjectCollection>();
            var gameObject = sceneManager[name];
            return gameObject;
        }

        public IGameObjectCollection GetGameObjectsByTag(string tag)
        {
            // todo: fix properly
            var sceneManager = Services.Get<IGameObjectCollection>();
            var gameObjects = sceneManager.FindByTag(tag);
            return gameObjects;
        }
    }
}
