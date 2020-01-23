﻿using System;
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

        public Input(PlayerIndex playerIndex, Dictionary<T, List<Buttons>> buttonMappings,
            Dictionary<T, List<Keys>> keyMappings)
        {
            GamePadIndex = playerIndex;
            LeftStick = Vector2.Zero;
            WasPressed = new Dictionary<T, bool>();
            IsPressed = new Dictionary<T, bool>();
            PressedTime = new Dictionary<T, float>();
            ButtonMappings = buttonMappings;
            KeyMappings = keyMappings;

            if (ButtonMappings != null)
                foreach (var kvp in ButtonMappings)
                {
                    WasPressed[kvp.Key] = false;
                    IsPressed[kvp.Key] = false;
                }

            if (KeyMappings != null)
                foreach (var kvp in KeyMappings)
                {
                    WasPressed[kvp.Key] = false;
                    IsPressed[kvp.Key] = false;
                }
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

        private bool AnyButtonDown(GamePadState state, List<Buttons> buttons)
        {
            foreach (var button in buttons)
                if (state.IsButtonDown(button))
                    return true;

            return false;
        }

        private bool AnyKeyDown(KeyboardState state, List<Keys> keys)
        {
            foreach (var key in keys)
                if (state.IsKeyDown(key))
                    return true;

            return false;
        }

        public bool AnyActionDown()
        {
            foreach (var action in IsPressed)
                if (action.Value)
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

                if (ButtonMappings != null && !ButtonMappings.ContainsKey(action))
                    ButtonMappings[action] = new List<Buttons>();

                if (KeyMappings != null && !KeyMappings.ContainsKey(action))
                    KeyMappings[action] = new List<Keys>();

                WasPressed[action] = IsPressed[action];

                if (ButtonMappings != null && AnyButtonDown(gamePad, ButtonMappings[action]) ||
                    KeyMappings != null && AnyKeyDown(keyboard, KeyMappings[action]))
                {
                    IsPressed[action] = true;
                    if (!PressedTime.ContainsKey(action))
                        PressedTime.Add(action, 0f);
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