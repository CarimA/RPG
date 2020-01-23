using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Transforms;
using PhotoVs.Logic.WorldZoning;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Collision
{
    internal class SProcessInteractionEvents : IUpdateableSystem
    {
        private readonly HashSet<IGameObject> _enteredScripts;
        private readonly Events _events;
        private readonly SMapBoundaryGeneration _mapBoundary;

        public SProcessInteractionEvents(Events events, SMapBoundaryGeneration mapBoundary)
        {
            _mapBoundary = mapBoundary;
            _events = events;
            _enteredScripts = new HashSet<IGameObject>();
        }

        public int Priority { get; set; } = -1;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = {typeof(CInput)};

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            var scripts = _mapBoundary.GetScripts();

            foreach (var entity in entities)
                HandleInteraction(entity, scripts);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void HandleInteraction(IGameObject entity, IGameObjectCollection scripts)
        {
            var input = entity.Components.Get<CInput>().Input;
            var velocity = entity.Components.Get<CVelocity>();

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

                    if (velocity.Velocity != Vector2.Zero)
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