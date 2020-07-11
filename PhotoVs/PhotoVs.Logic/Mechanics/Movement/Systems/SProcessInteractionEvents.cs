using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Events;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Logic.PlayerData;
using System;
using System.Collections.Generic;
using PhotoVs.Engine;
using PhotoVs.Engine.Events;

namespace PhotoVs.Logic.Mechanics.Movement.Systems
{
    internal class SProcessInteractionEvents : IUpdateableSystem
    {
        private readonly HashSet<GameObject> _enteredScripts;
        private readonly EventQueue<GameEvents> _events;
        private readonly SCamera _camera;
        private readonly Overworld _overworld;

        public SProcessInteractionEvents(Services services)
        {
            _camera = services.Get<SCamera>();
            _overworld = services.Get<Overworld>();
            _events = services.Get<EventQueue<GameEvents>>();
            _enteredScripts = new HashSet<GameObject>();
        }

        public int Priority { get; set; } = -1;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInputState) };

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
                        _events.Notify(GameEvents.InteractAction, scriptName, new InteractEventArgs(this, player, script));

                    if (!_enteredScripts.Contains(script))
                    {
                        _enteredScripts.Add(script);
                        _events.Notify(GameEvents.InteractAreaEnter, scriptName, new InteractEventArgs(this, player, script));
                    }

                    if (position.DeltaPosition != Vector2.Zero)
                    {
                        // todo: make it only run every x ticks to simulate footsteps
                        if (input.ActionDown(InputActions.Run))
                        {
                            if (player.CanMove)
                                // todo: should only fire on footstep touching ground
                                _events.Notify(GameEvents.InteractAreaRun, scriptName, new InteractEventArgs(this, player, script));
                        }
                        else
                        {
                            if (player.CanMove)
                                _events.Notify(GameEvents.InteractAreaWalk, scriptName, new InteractEventArgs(this, player, script));
                        }
                    }
                    else
                    {
                        if (player.CanMove)
                            _events.Notify(GameEvents.InteractAreaStand, scriptName, new InteractEventArgs(this, player, script));
                    }
                }
                else
                {
                    if (!_enteredScripts.Contains(script))
                        continue;

                    _enteredScripts.Remove(script);
                    _events.Notify(GameEvents.InteractAreaExit, scriptName, new InteractEventArgs(this, player, script));
                }
            }
        }
    }
}