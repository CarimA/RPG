﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class CController
    {
        public CController(PlayerIndex playerIndex, Dictionary<InputActions, List<Buttons>> buttonMappings,
            float deadzone)
        {
            PlayerIndex = playerIndex;
            ButtonMappings = buttonMappings;
            Deadzone = deadzone;

            var inputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
            foreach (var input in inputActions)
                if (!ButtonMappings.ContainsKey(input))
                    ButtonMappings.Add(input, new List<Buttons>());
        }

        public Dictionary<InputActions, List<Buttons>> ButtonMappings { get; }
        public PlayerIndex PlayerIndex { get; }
        public float Deadzone { get; set; }

        public bool AnyButtonDown(GamePadState state, IEnumerable<Buttons> buttons)
        {
            foreach (var button in buttons)
                if (state.IsButtonDown(button))
                    return true;

            return false;
        }

        public bool AnyButtonDown(GamePadState state)
        {
            return AnyButtonDown(state, Enum.GetValues(typeof(Buttons)).Cast<Buttons>());
        }
    }
}