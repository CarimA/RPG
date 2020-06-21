﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Mechanics.Input.Components
{
    public enum InputPriority
    {
        GamePad,
        Keyboard
    }

    public class CInputState : IComponent
    {
        public Dictionary<InputActions, bool> IsPressed { get; }
        public Dictionary<InputActions, float> PressedTime { get; }
        public Dictionary<InputActions, bool> WasPressed { get; }

        public Vector2 LeftAxis { get; set; }
        public Vector2 RightAxis { get; set; }

        public InputPriority InputPriority { get; set; }

        public CInputState()
        {
            IsPressed = new Dictionary<InputActions, bool>();
            PressedTime = new Dictionary<InputActions, float>();
            WasPressed = new Dictionary<InputActions, bool>();

            var inputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
            foreach (var input in inputActions)
            {
                WasPressed[input] = false;
                IsPressed[input] = false;
                PressedTime[input] = 0;
            }

            LeftAxis = Vector2.Zero;
            RightAxis = Vector2.Zero;
            InputPriority = InputPriority.GamePad;
        }

        public bool ActionPressed(InputActions action)
        {
            if (!IsPressed.ContainsKey(action) || !WasPressed.ContainsKey(action))
                return false;

            return IsPressed[action] && !WasPressed[action];
        }

        public bool ActionReleased(InputActions action)
        {
            if (!IsPressed.ContainsKey(action) || !WasPressed.ContainsKey(action))
                return false;

            return !IsPressed[action] && WasPressed[action];
        }

        public bool ActionDown(InputActions action)
        {
            return IsPressed.ContainsKey(action) && IsPressed[action];
        }

        public bool ActionUp(InputActions action)
        {
            if (!IsPressed.ContainsKey(action))
                return false;

            return !IsPressed[action];
        }

        public float ActionPressedTime(InputActions action)
        {
            return PressedTime[action];
        }

        public bool AnyActionDown()
        {
            foreach (var action in IsPressed)
                if (action.Value)
                    return true;

            return false;
        }
    }
}