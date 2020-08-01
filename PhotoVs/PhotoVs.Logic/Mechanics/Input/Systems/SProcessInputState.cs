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
    public class SProcessInputState : IUpdateableSystem
    {
        private readonly IEnumerable<InputActions> AllInputActions;

        public SProcessInputState()
        {
            AllInputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
        }

        public int Priority { get; set; } = -1000;
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
            entities.ForEach(ProcessInput);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void ProcessInput(GameObject gameObject)
        {
            var inputState = gameObject.Components.Get<CInputState>();
            ResetIfMissing(gameObject, inputState);
            ProcessState(inputState);
            CheckPriority(gameObject);
        }

        private void ResetIfMissing(GameObject gameObject, CInputState inputState)
        {
            if (gameObject.Components.Has<CKeyboard>() && gameObject.Components.Has<CController>())
                return;

            foreach (var input in AllInputActions)
            {
                inputState.IsPressed[input] = false;
                inputState.PressedTime[input] = 0;
            }
        }

        private void ProcessState(CInputState inputState)
        {
            foreach (var input in AllInputActions) inputState.WasPressed[input] = inputState.IsPressed[input];
        }

        private void CheckPriority(GameObject gameObject)
        {
            if (!gameObject.Components.TryGet<CInputPriority>(out var prio))
                return;

            var priority = prio.InputPriority;

            // if both components exist, give it to the one that has input if the other does not.
            // if both components exist and both are pushing inputs, keep it on whatever had it last frame
            var anyKeyDown = false;
            var anyButtonDown = false;

            // if either component is missing, give it to the other.
            if (!gameObject.Components.TryGet<CKeyboard>(out var keyboard))
                priority = InputPriority.GamePad;

            if (!gameObject.Components.TryGet<CController>(out var controller))
                priority = InputPriority.Keyboard;

            if (controller != null) anyKeyDown = keyboard.AnyKeyDown(Keyboard.GetState());

            if (controller != null) anyButtonDown = controller.AnyButtonDown(GamePad.GetState(controller.PlayerIndex));

            if (anyKeyDown && !anyButtonDown)
                priority = InputPriority.Keyboard;

            if (anyButtonDown && !anyKeyDown)
                priority = InputPriority.GamePad;

            prio.InputPriority = priority;
        }
    }
}