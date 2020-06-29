﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Logic.Mechanics.Input.Components
{
    public class CKeyboard
    {
        public Dictionary<InputActions, List<Keys>> KeyMappings { get; }

        public CKeyboard(Dictionary<InputActions, List<Keys>> keyMappings)
        {
            KeyMappings = keyMappings;
            var inputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
            foreach (var input in inputActions)
            {
                if (!KeyMappings.ContainsKey(input))
                    KeyMappings.Add(input, new List<Keys>());
            }
        }

        public bool AnyKeyDown(KeyboardState state, IEnumerable<Keys> keys)
        {
            foreach (var key in keys)
                if (state.IsKeyDown(key))
                    return true;

            return false;
        }

        public bool AnyKeyDown(KeyboardState state)
        {
            return AnyKeyDown(state, Enum.GetValues(typeof(Keys)).Cast<Keys>());
        }
    }
}
