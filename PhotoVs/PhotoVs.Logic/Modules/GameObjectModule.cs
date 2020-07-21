using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using PhotoVs.Logic.NewScenes;

namespace PhotoVs.Logic.Modules
{
    public class GameObjectModule : Module
    {
        private readonly SceneMachine _sceneMachine;
        private readonly Player _player;
        private GameTime _latestGameTime;

        public GameObjectModule(SceneMachine sceneMachine, Player player)
        {
            _sceneMachine = sceneMachine;
            _player = player;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("Player", (Func<string>)GetPlayer);
            interpreter.AddFunction("GameObject", (Func<string, string>)GetGameObjectByName);
            interpreter.AddFunction("GameObjectsByTag", (Func<string, IEnumerable<string>>)GetGameObjectsByTag);

            interpreter.AddFunction("_Move", (Func<string, Vector2, float, bool>)Move);

            interpreter.AddType<Vector2>("Vector2");
            interpreter.RegisterGlobal("Vector2", (Func<float, float, Vector2>)CreateVector2);

            interpreter.AddFunction("Warp", (Action<string, Vector2>)Warp);

            base.DefineApi(interpreter);
        }

        private void Warp(string gameObjectId, Vector2 position)
        {
            var gameObject = _sceneMachine.GameObjects[gameObjectId];
            var cPosition = gameObject.Components.Get<CPosition>();
            cPosition.Position = position;
            cPosition.LastPosition = position;
        }

        private Vector2 CreateVector2(float arg1, float arg2)
        {
            return new Vector2(arg1, arg2);
        }

        public override void Update(GameTime gameTime)
        {
            _latestGameTime = gameTime;
            base.Update(gameTime);
        }

        private string GetPlayer()
        {
            return _player.ID;
        }

        private IEnumerable<string> GetGameObjectsByTag(string tag)
        {
            return _sceneMachine.GameObjects.HasTag(tag).Select(gameObject => gameObject.ID);
        }

        private string GetGameObjectByName(string name)
        {
            return _sceneMachine.GameObjects[name].ID;
        }

        private bool Move(string gameObjectId, Vector2 target, float speed)
        {
            var gameObject = _sceneMachine.GameObjects[gameObjectId];
            var position = gameObject.Components.Get<CPosition>();
            var input = gameObject.Components.Get<CInputState>();
            var amount = speed * _latestGameTime.GetElapsedSeconds();

            if (Vector2.Distance(position.Position, target) < amount)
            {
                position.Position = target;
                SetInputs(gameObject, true);
                input.LeftAxis = Vector2.Zero;
                return false;
            }
            else
            {
                var direction = target - position.Position;
                direction.Normalize();

                input.LeftAxis = direction;
                SetInputs(gameObject, false);

                return true;
            }
        }

        private void SetInputs(GameObject gameObject, bool state)
        {
            if (state)
            {
                gameObject.Components.Enable<CController>();
                gameObject.Components.Enable<CKeyboard>();
            }
            else
            {
                gameObject.Components.Disable<CController>();
                gameObject.Components.Disable<CKeyboard>();
            }
        }

        /*private string Say(string gameObjectId, string dialogue)
        {
            // todo: migrate dialogue module here
        }*/
    }
}
