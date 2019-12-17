using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhotoVs.GameInput
{
    public class Input
    {
        public Dictionary<InputActions, List<Buttons>> ButtonMappings;
        public PlayerIndex GamePadIndex;
        public Dictionary<InputActions, bool> IsPressed;
        public Dictionary<InputActions, List<Keys>> KeyMappings;
        public Vector2 LeftStick;
        public Dictionary<InputActions, float> PressedTime;

        public Dictionary<InputActions, bool> WasPressed;

        public Input(PlayerIndex playerIndex)
        {
            GamePadIndex = playerIndex;
            LeftStick = Vector2.Zero;
            WasPressed = new Dictionary<InputActions, bool>();
            IsPressed = new Dictionary<InputActions, bool>();
            PressedTime = new Dictionary<InputActions, float>();
            ButtonMappings = new Dictionary<InputActions, List<Buttons>>();
            KeyMappings = new Dictionary<InputActions, List<Keys>>();
        }

        public Vector2 GetAxis()
        {
            var output = LeftStick;

            if (output != Vector2.Zero)
                return output;

            output.Y -= ActionDown(InputActions.Up) ? 1f : 0;
            output.Y += ActionDown(InputActions.Down) ? 1f : 0;
            output.X -= ActionDown(InputActions.Left) ? 1f : 0;
            output.X += ActionDown(InputActions.Right) ? 1f : 0;

            if (output != Vector2.Zero)
                output.Normalize();

            return output;
        }

        public bool ActionPressed(InputActions action)
        {
            if (!IsPressed.ContainsKey(action) || !WasPressed.ContainsKey(action)) return false;

            return IsPressed[action] && !WasPressed[action];
        }

        public bool ActionReleased(InputActions action)
        {
            if (!IsPressed.ContainsKey(action) || !WasPressed.ContainsKey(action)) return false;

            return !IsPressed[action] && WasPressed[action];
        }

        public bool ActionDown(InputActions action)
        {
            return IsPressed.ContainsKey(action) && IsPressed[action];
        }

        public bool ActionUp(InputActions action)
        {
            if (!IsPressed.ContainsKey(action)) return false;

            return !IsPressed[action];
        }

        public float ActionPressedTime(InputActions action)
        {
            return PressedTime[action];
        }

        public bool AnyButtonDown(GamePadState state, List<Buttons> buttons)
        {
            foreach (var button in buttons)
                if (state.IsButtonDown(button))
                    return true;

            return false;
        }

        public bool AnyKeyDown(KeyboardState state, List<Keys> keys)
        {
            foreach (var key in keys)
                if (state.IsKeyDown(key))
                    return true;

            return false;
        }

        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var gamePad = GamePad.GetState(GamePadIndex);

            foreach (InputActions action in Enum.GetValues(typeof(InputActions)))
            {
                if (!IsPressed.ContainsKey(action)) IsPressed[action] = false;

                if (!ButtonMappings.ContainsKey(action)) ButtonMappings[action] = new List<Buttons>();

                if (!KeyMappings.ContainsKey(action)) KeyMappings[action] = new List<Keys>();

                WasPressed[action] = IsPressed[action];

                if (AnyButtonDown(gamePad, ButtonMappings[action]) ||
                    AnyKeyDown(keyboard, KeyMappings[action]))
                {
                    IsPressed[action] = true;
                    PressedTime[action] += (float) gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    IsPressed[action] = false;
                    PressedTime[action] = 0;
                }
            }

            LeftStick = gamePad.ThumbSticks.Left * new Vector2(1, -1);
        }
    }
}