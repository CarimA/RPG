using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics
{
    public class Input
    {
        private readonly ISignal _signal;
        private readonly IEnumerable<InputActions> _allInputActions;
        private readonly Vector2 _inverseY;

        public Input(ISignal signal)
        {
            _signal = signal;
            _allInputActions = Enum.GetValues(typeof(InputActions)).Cast<InputActions>();
            _inverseY = new Vector2(1, -1);
        }

        [System(RunOn.Update, typeof(CInputState), typeof(CKeyboard))]
        public void ProcessKeyboards(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
                ProcessKeyboard(gameTime, gameObject);
        }

        private void ProcessKeyboard(GameTime gameTime, GameObject gameObject)
        {
            var inputState = gameObject.Components.Get<CInputState>();

            if (gameObject.Components.TryGet<CInputPriority>(out var priority))
                if (priority.InputPriority != InputPriority.Keyboard)
                    return;

            var keyboard = gameObject.Components.Get<CKeyboard>();
            var keyInput = Keyboard.GetState();

            foreach (var action in _allInputActions)
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

        [System(RunOn.Update, typeof(CInputState), typeof(CController))]
        public void ProcessControllers(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
                ProcessController(gameTime, gameObject);
        }

        private void ProcessController(GameTime gameTime, GameObject gameObject)
        {
            if (gameObject.Components.TryGet<CInputPriority>(out var priority))
                if (priority.InputPriority != InputPriority.GamePad)
                    return;

            var inputState = gameObject.Components.Get<CInputState>();
            var controller = gameObject.Components.Get<CController>();
            var gamePad = GamePad.GetState(controller.PlayerIndex);

            foreach (var action in _allInputActions)
                if (controller.AnyButtonDown(gamePad, controller.ButtonMappings[action]))
                {
                    inputState.IsPressed[action] = true;
                    inputState.PressedTime[action] += gameTime.GetElapsedSeconds();
                }
                else
                {
                    inputState.IsPressed[action] = false;
                    inputState.PressedTime[action] = 0;
                }

            inputState.LeftAxis = GetAxisWithDeadzone(gamePad.ThumbSticks.Left, controller.Deadzone);
            inputState.RightAxis = GetAxisWithDeadzone(gamePad.ThumbSticks.Right, controller.Deadzone);
        }

        private Vector2 GetAxisWithDeadzone(Vector2 inputAxis, float deadzone)
        {
            var stick = inputAxis * _inverseY;
            var length = stick.Length();

            if (length >= 1f)
                length = 1f;

            if (length < deadzone)
            {
                return Vector2.Zero;
            }

            var deadzoneAdjusted = stick * ((length - deadzone) / (1f - deadzone));
            return deadzoneAdjusted;
        }

        [System(RunOn.Update, typeof(CInputState))]
        public void RaiseEvents(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var inputState = gameObject.Components.Get<CInputState>();

                foreach (var action in _allInputActions)
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

        [System(RunOn.Update, typeof(CInputState))]
        public void ResetInputSchemesIfDisconnected(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                // todo: make sure dummy inputs don't get handled by this system
                if (gameObject.Components.Has<CKeyboard>() && gameObject.Components.Has<CController>())
                    return;

                var inputState = gameObject.Components.Get<CInputState>();

                foreach (var input in _allInputActions)
                {
                    inputState.IsPressed[input] = false;
                    inputState.PressedTime[input] = 0;
                }
            }
        }

        [System(RunOn.Update, typeof(CInputState))]
        public void SetLastState(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var inputState = gameObject.Components.Get<CInputState>();

                foreach (var input in _allInputActions)
                    inputState.WasPressed[input] = inputState.IsPressed[input];
            }
        }

        [System(RunOn.Update, typeof(CInputPriority))]
        public void CheckPriority(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var inputPriority = gameObject.Components.Get<CInputPriority>();
                var priority = inputPriority.InputPriority;

                // if both components exist, give it to the one that has input if the other does not.
                // if both components exist and both are pushing inputs, keep it on whatever had it last frame
                var anyKeyDown = false;
                var anyButtonDown = false;

                // if either component is missing, give it to the other.
                if (!gameObject.Components.TryGet<CKeyboard>(out var keyboard))
                    priority = InputPriority.GamePad;

                if (!gameObject.Components.TryGet<CController>(out var controller))
                    priority = InputPriority.Keyboard;

                if (controller != null)
                    anyKeyDown = keyboard.AnyKeyDown(Keyboard.GetState());

                if (controller != null)
                    anyButtonDown = controller.AnyButtonDown(GamePad.GetState(controller.PlayerIndex));

                if (anyKeyDown && !anyButtonDown)
                    priority = InputPriority.Keyboard;

                if (anyButtonDown && !anyKeyDown)
                    priority = InputPriority.GamePad;

                inputPriority.InputPriority = priority;
            }
        }
    }
}
