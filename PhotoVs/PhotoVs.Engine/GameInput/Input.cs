using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhotoVs.Engine.GameInput
{
    public class Input<T> where T : Enum
    {
        public Dictionary<T, List<Buttons>> ButtonMappings;
        public PlayerIndex GamePadIndex;
        public Dictionary<T, bool> IsPressed;
        public Dictionary<T, List<Keys>> KeyMappings;
        public Vector2 LeftStick;
        public Dictionary<T, float> PressedTime;

        public Dictionary<T, bool> WasPressed;

        public Input(PlayerIndex playerIndex)
        {
            GamePadIndex = playerIndex;
            LeftStick = Vector2.Zero;
            WasPressed = new Dictionary<T, bool>();
            IsPressed = new Dictionary<T, bool>();
            PressedTime = new Dictionary<T, float>();
            ButtonMappings = new Dictionary<T, List<Buttons>>();
            KeyMappings = new Dictionary<T, List<Keys>>();
        }

        public bool ActionPressed(T action)
        {
            if (!IsPressed.ContainsKey(action) || !WasPressed.ContainsKey(action))
                return false;

            return IsPressed[action] && !WasPressed[action];
        }

        public bool ActionReleased(T action)
        {
            if (!IsPressed.ContainsKey(action) || !WasPressed.ContainsKey(action))
                return false;

            return !IsPressed[action] && WasPressed[action];
        }

        public bool ActionDown(T action)
        {
            return IsPressed.ContainsKey(action) && IsPressed[action];
        }

        public bool ActionUp(T action)
        {
            if (!IsPressed.ContainsKey(action))
                return false;

            return !IsPressed[action];
        }

        public float ActionPressedTime(T action)
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

            foreach (T action in Enum.GetValues(typeof(T)))
            {
                if (!IsPressed.ContainsKey(action))
                    IsPressed[action] = false;

                if (!ButtonMappings.ContainsKey(action))
                    ButtonMappings[action] = new List<Buttons>();

                if (!KeyMappings.ContainsKey(action))
                    KeyMappings[action] = new List<Keys>();

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