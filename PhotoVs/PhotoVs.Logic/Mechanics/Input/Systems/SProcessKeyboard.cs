using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.Input.Systems
{
    public class SProcessKeyboard : IUpdateableSystem
    {
        public int Priority { get; set; } = -999;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
            typeof(CInputState),
            typeof(CKeyboard)
        };

        private readonly IEnumerable<InputActions> AllInputActions;

        public SProcessKeyboard()
        {
            AllInputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
        }

        public void BeforeUpdate(GameTime gameTime)
        {

        }

        public void Update(GameTime gameTime, GameObjectList entities)
        {
            foreach (var gameObject in entities)
            {
                ProcessInput(gameObject, gameTime);
            }
        }

        private void ProcessInput(GameObject gameObject, GameTime gameTime)
        {
            var inputState = gameObject.Components.Get<CInputState>();

            if (gameObject.Components.TryGet<CInputPriority>(out var priority))
                if (priority.InputPriority != InputPriority.Keyboard)
                    return;

            var keyboard = gameObject.Components.Get<CKeyboard>();
            var keyInput = Keyboard.GetState();

            foreach (InputActions action in AllInputActions)
            {
                if (keyboard != null && keyboard.AnyKeyDown(keyInput, keyboard.KeyMappings[action]))
                {
                    inputState.IsPressed[action] = true;
                    inputState.PressedTime[action] += gameTime.GetElapsedSeconds();
                }
                else
                {
                    inputState.IsPressed[action] = false;
                    inputState.PressedTime[action] = 0;
                }
            }

            inputState.LeftAxis = GetAxis(inputState, InputActions.Up, InputActions.Down, InputActions.Left,
                InputActions.Right);
            inputState.RightAxis = Vector2.Zero;
        }

        private Vector2 GetAxis(CInputState inputState, InputActions up, InputActions down, InputActions left,
            InputActions right)
        {
            var output = Vector2.Zero;

            output.Y -= inputState.ActionDown(up) ? 1f : 0;
            output.Y += inputState.ActionDown(down) ? 1f : 0;
            output.X -= inputState.ActionDown(left) ? 1f : 0;
            output.X += inputState.ActionDown(right) ? 1f : 0;

            if (output != Vector2.Zero)
                output.Normalize();

            return output;
        }

        public void AfterUpdate(GameTime gameTime)
        {

        }
    }
}
