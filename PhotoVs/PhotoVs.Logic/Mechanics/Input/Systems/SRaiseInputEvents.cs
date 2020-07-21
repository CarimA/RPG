using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Events;
using PhotoVs.Logic.Mechanics.Input.Components;

namespace PhotoVs.Logic.Mechanics.Input.Systems
{
    public class SRaiseInputEvents : IUpdateableSystem
    {
        public int Priority { get; set; } = -998;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
            typeof(CInputState)
        };

        private readonly GameEventQueue _eventQueue;
        private readonly IEnumerable<InputActions> AllInputActions;

        public SRaiseInputEvents(Services services)
        {
            AllInputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
            _eventQueue = services.Get<GameEventQueue>();
        }

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
                        _eventQueue.Notify(GameEvents.InputActionPressed, 
                            Enum.GetName(typeof(InputActions), action), 
                            new GameObjectEventArgs(this, gameObject));

                    if (inputState.ActionReleased(action))
                        _eventQueue.Notify(GameEvents.InputActionReleased,
                            Enum.GetName(typeof(InputActions), action),
                            new GameObjectEventArgs(this, gameObject));
                }
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {

        }
    }
}
