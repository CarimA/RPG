using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.Input.Systems
{
    public class SProcessInputState : IUpdateableSystem
    {
        public int Priority { get; set; } = -1000;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
            typeof(CInputState)
        };

        private readonly IEnumerable<InputActions> AllInputActions;    

        public SProcessInputState()
        {
            AllInputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
        }

        public void BeforeUpdate(GameTime gameTime)
        {

        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            entities.ForEach(ProcessInput);
        }

        private void ProcessInput(IGameObject gameObject)
        {
            var inputState = gameObject.Components.Get<CInputState>();
            ResetIfMissing(gameObject, inputState);
            ProcessState(inputState);
            CheckPriority(gameObject, inputState);
        }

        private void ResetIfMissing(IGameObject gameObject, CInputState inputState)
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
            foreach (var input in AllInputActions)
            {
                inputState.WasPressed[input] = inputState.IsPressed[input];
            }
        }

        private void CheckPriority(IGameObject gameObject, CInputState inputState)
        {
            var priority = inputState.InputPriority;

            // if either component is missing, give it to the other.
            if (!gameObject.Components.TryGet<CKeyboard>(out var keyboard))
                priority = InputPriority.GamePad;

            if (!gameObject.Components.TryGet<CController>(out var controller))
                priority = InputPriority.Keyboard;

            // if both components exist, give it to the one that has input if the other does not.
            // if both components exist and both are pushing inputs, keep it on whatever had it last frame
            var anyKeyDown = false;
            if (controller != null)
            {
                anyKeyDown = keyboard.AnyKeyDown(Keyboard.GetState());
            }

            var anyButtonDown = false;
            if (controller != null)
            {
                anyButtonDown = controller.AnyButtonDown(GamePad.GetState(controller.PlayerIndex));
            }

            if (anyKeyDown && !anyButtonDown)
                priority = InputPriority.Keyboard;

            if (anyButtonDown && !anyKeyDown)
                priority = InputPriority.GamePad;

            inputState.InputPriority = priority;
        }

        public void AfterUpdate(GameTime gameTime)
        {

        }
    }
}