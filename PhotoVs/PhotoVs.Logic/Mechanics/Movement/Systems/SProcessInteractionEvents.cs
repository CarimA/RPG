﻿using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Logic.PlayerData;
using System;
using System.Collections.Generic;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.Mechanics.Movement.Systems
{
    internal class SProcessInteractionEvents : IUpdateableSystem
    {
        private readonly HashSet<IGameObject> _enteredScripts;
        private readonly Events _events;
        private readonly SCamera _camera;
        private readonly Overworld _overworld;

        public SProcessInteractionEvents(Events events, SCamera camera, Overworld overworld)
        {
            _camera = camera;
            _overworld = overworld;
            _events = events;
            _enteredScripts = new HashSet<IGameObject>();
        }

        public int Priority { get; set; } = -1;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInput) };

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            var scripts = _overworld.GetMap().GetScripts(_camera);

            foreach (var entity in entities)
                HandleInteraction(entity, scripts);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void HandleInteraction(IGameObject entity, IGameObjectCollection scripts)
        {
            var input = entity.Components.Get<CInput>().Input;
            var position = entity.Components.Get<CPosition>();

            if (!(entity is Player player))
                return;

            if (!player.CanMove)
                return;

            foreach (var script in scripts)
            {
                var scriptName = script.Components.Get<CScript>().Name;
                var result = CollisionResult.Simulate(entity, script, Vector2.Zero);
                if (result.AreIntersecting)
                {
                    if (input.ActionPressed(InputActions.Action))
                        _events.RaiseOnInteractEventAction(scriptName, player, script);

                    if (!_enteredScripts.Contains(script))
                    {
                        _enteredScripts.Add(script);
                        _events.RaiseOnInteractEventEnter(scriptName, player, script);
                    }

                    if (position.DeltaPosition != Vector2.Zero)
                    {
                        // todo: make it only run every x ticks to simulate footsteps
                        if (input.ActionDown(InputActions.Run))
                        {
                            if (player.CanMove)
                                // todo: should only fire on footstep touching ground
                                _events.RaiseOnInteractEventRun(scriptName, player, script);
                        }
                        else
                        {
                            if (player.CanMove)
                                _events.RaiseOnInteractEventWalk(scriptName, player, script);
                        }
                    }
                    else
                    {
                        if (player.CanMove)
                            _events.RaiseOnInteractEventStand(scriptName, player, script);
                    }
                }
                else
                {
                    if (_enteredScripts.Contains(script))
                    {
                        _enteredScripts.Remove(script);
                        _events.RaiseOnInteractEventExit(scriptName, player, script);
                    }
                }
            }
        }
    }
}