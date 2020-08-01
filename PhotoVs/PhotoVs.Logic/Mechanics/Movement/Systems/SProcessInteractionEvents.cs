using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic.Mechanics.Movement.Systems
{
    internal class SProcessInteractionEvents : IUpdateableSystem
    {
        private readonly SCamera _camera;
        private readonly HashSet<GameObject> _enteredScripts;
        private readonly IOverworld _overworld;
        private readonly ISignal _signal;

        public SProcessInteractionEvents(IGameState gameState, IOverworld overworld, ISignal signal)
        {
            _overworld = overworld;
            _signal = signal;
            _camera = gameState.Camera;

            _enteredScripts = new HashSet<GameObject>();
        }

        public int Priority { get; set; } = -1;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = {typeof(CInputState)};

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, GameObjectList entities)
        {
            var scripts = _overworld.GetMap().GetScripts(_camera);

            foreach (var entity in entities)
                HandleInteraction(entity, scripts);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void HandleInteraction(GameObject entity, IEnumerable<GameObject> scripts)
        {
            var input = entity.Components.Get<CInputState>();
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
                        _signal.Notify($"InteractAction:{scriptName}", new InteractEventArgs(this, player, script));

                    if (!_enteredScripts.Contains(script))
                    {
                        _enteredScripts.Add(script);
                        _signal.Notify($"InteractAreaEnter:{scriptName}", new InteractEventArgs(this, player, script));
                    }

                    if (position.DeltaPosition != Vector2.Zero)
                    {
                        // todo: make it only run every x ticks to simulate footsteps
                        if (input.ActionDown(InputActions.Run))
                        {
                            if (player.CanMove)
                                // todo: should only fire on footstep touching ground
                                _signal.Notify($"InteractAreaRun:{scriptName}",
                                    new InteractEventArgs(this, player, script));
                        }
                        else
                        {
                            if (player.CanMove)
                                _signal.Notify($"InteractAreaWalk:{scriptName}",
                                    new InteractEventArgs(this, player, script));
                        }
                    }
                    else
                    {
                        if (player.CanMove)
                            _signal.Notify($"InteractAreaStand:{scriptName}",
                                new InteractEventArgs(this, player, script));
                    }
                }
                else
                {
                    if (!_enteredScripts.Contains(script))
                        continue;

                    _enteredScripts.Remove(script);
                    _signal.Notify($"InteractAreaExit:{scriptName}", new InteractEventArgs(this, player, script));
                }
            }
        }
    }
}