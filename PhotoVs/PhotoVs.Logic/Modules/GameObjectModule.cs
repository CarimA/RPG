﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Modules
{
    public class GameObjectModule : Module
    {
        private readonly IGameObjectCollection _collection;
        private readonly Player _player;
        private GameTime _latestGameTime;

        public GameObjectModule(IGameObjectCollection collection, Player player)
        {
            _collection = collection;
            _player = player;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("player", (Func<string>)GetPlayer);
            interpreter.AddFunction("find_game_object", (Func<string, string>)GetGameObjectByName);
            interpreter.AddFunction("find_game_objects_with_tag", (Func<string, IEnumerable<string>>)GetGameObjectsByTag);

            interpreter.AddFunction("_move", (Func<string, Vector2, float, bool>)Move);
            interpreter.RunScript($@"
                function move(g, t, s)
                    while _move(g, t, s) do
                        coroutine.yield()
                    end
                end");

            UserData.RegisterType<Vector2>();
            interpreter.RegisterGlobal("vec2", (Func<float, float, Vector2>)CreateVector2);

            base.DefineApi(interpreter);
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
            return _collection.FindByTag(tag).Select(gameObject => gameObject.ID);
        }

        private string GetGameObjectByName(string name)
        {
            return _collection[name].ID;
        }

        private bool Move(string gameObjectId, Vector2 target, float speed)
        {
            var gameObject = _collection.FindById(gameObjectId);
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

        private void SetInputs(IGameObject gameObject, bool state)
        {
            if (gameObject.Components.TryGet<CController>(out var controller))
                controller.Enabled = state;

            if (gameObject.Components.TryGet<CKeyboard>(out var keyboard))
                keyboard.Enabled = state;
        }

        /*private string Say(string gameObjectId, string dialogue)
        {
            // todo: migrate dialogue module here
        }*/
    }
}