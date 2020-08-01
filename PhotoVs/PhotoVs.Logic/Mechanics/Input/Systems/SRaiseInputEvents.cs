using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.Input.Components;

namespace PhotoVs.Logic.Mechanics.Input.Systems
{
    public class SRaiseInputEvents : IUpdateableSystem
    {
        private readonly ISignal _signal;
        private readonly IEnumerable<InputActions> AllInputActions;

        public SRaiseInputEvents(ISignal signal)
        {
            AllInputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
            _signal = signal;
        }

        public int Priority { get; set; } = -998;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
            typeof(CInputState)
        };

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, GameObjectList entities)
        {
            foreach (var gameObject in entities)
            {
                var inputState = gameObject.Components.Get<CInputState>();

                foreach (var action in AllInputActions)
                {
                    if (inputState.ActionPressed(action))
                        _signal.Notify($"InputActionPressed:{Enum.GetName(typeof(InputActions), action)}",
                            new GameObjectEventArgs(this, gameObject));

                    if (inputState.ActionReleased(action))
                        _signal.Notify($"InputActionReleased:{Enum.GetName(typeof(InputActions), action)}",
                            new GameObjectEventArgs(this, gameObject));
                }
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}