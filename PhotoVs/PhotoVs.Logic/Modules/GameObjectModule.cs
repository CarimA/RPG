using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Modules
{
    public class GameObjectModule
    {
        private readonly GameState _gameState;
        private readonly Stage _stage;

        public GameObjectModule(IInterpreter<Closure> interpreter, GameState gameState, Stage stage)
        {
            _gameState = gameState;
            _stage = stage;

            interpreter.AddFunction("Player", (Func<string>) GetPlayer);
            interpreter.AddFunction("GameObject", (Func<string, string>) GetGameObjectByName);
            interpreter.AddFunction("GameObjectsByTag", (Func<string, IEnumerable<string>>) GetGameObjectsByTag);

            interpreter.AddFunction("_Move", (Func<string, Vector2, float, bool>) Move);

            interpreter.AddType<Vector2>("Vector2");
            interpreter.RegisterGlobal("Vector2", (Func<float, float, Vector2>) CreateVector2);

            interpreter.AddFunction("Warp", (Action<string, Vector2>) Warp);
        }

        private void Warp(string gameObjectId, Vector2 position)
        {
            var gameObject = _stage.GameObjects[gameObjectId];
            var cPosition = gameObject.Components.Get<CPosition>();
            cPosition.Position = position;
            cPosition.LastPosition = position;
        }

        private Vector2 CreateVector2(float arg1, float arg2)
        {
            return new Vector2(arg1, arg2);
        }

        private string GetPlayer()
        {
            return _gameState.Player.ID;
        }

        private IEnumerable<string> GetGameObjectsByTag(string tag)
        {
            return _stage.GameObjects.HasTag(tag).Select(gameObject => gameObject.ID);
        }

        private string GetGameObjectByName(string name)
        {
            return _stage.GameObjects[name].ID;
        }

        private bool Move(string gameObjectId, Vector2 target, float speed)
        {
            var gameObject = _stage.GameObjects[gameObjectId];
            var position = gameObject.Components.Get<CPosition>();
            var input = gameObject.Components.Get<CInputState>();
            var amount = speed * _gameState.GameTime.GetElapsedSeconds();

            if (Vector2.Distance(position.Position, target) < amount)
            {
                position.Position = target;
                SetInputs(gameObject, true);
                input.LeftAxis = Vector2.Zero;
                return false;
            }

            var direction = target - position.Position;
            direction.Normalize();

            input.LeftAxis = direction;
            SetInputs(gameObject, false);

            return true;
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