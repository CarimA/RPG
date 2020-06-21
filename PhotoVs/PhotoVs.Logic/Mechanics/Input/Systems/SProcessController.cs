﻿using System;
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
    public class SProcessController : IUpdateableSystem
    {
        public int Priority { get; set; } = -999;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
            typeof(CInputState),
            typeof(CController)
        };

        private readonly IEnumerable<InputActions> AllInputActions;
        private readonly Vector2 InverseY;

        public SProcessController()
        {
            AllInputActions = Enum.GetValues(typeof(InputActions))
                .Cast<InputActions>();
            InverseY = new Vector2(1, -1);
        }

        public void BeforeUpdate(GameTime gameTime)
        {

        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var gameObject in entities)
            {
                ProcessInput(gameObject, gameTime);
            }
        }

        private void ProcessInput(IGameObject gameObject, GameTime gameTime)
        {
            var inputState = gameObject.Components.Get<CInputState>();

            if (inputState.InputPriority != InputPriority.GamePad)
                return;

            var controller = gameObject.Components.Get<CController>();
            var gamePad = GamePad.GetState(controller.PlayerIndex);

            foreach (InputActions action in AllInputActions)
            {
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
            }

            inputState.LeftAxis = GetAxisWithDeadzone(gamePad.ThumbSticks.Left, controller.Deadzone);
            inputState.RightAxis = GetAxisWithDeadzone(gamePad.ThumbSticks.Right, controller.Deadzone);
        }

        private Vector2 GetAxisWithDeadzone(Vector2 inputAxis, float deadzone)
        {
            var stick = inputAxis * InverseY;
            var length = stick.Length();

            if (length >= 1f)
                length = 1f;

            if (length < deadzone)
                return Vector2.Zero;
            else
            {
                var deadzoneAdjusted = stick * ((length - deadzone) / (1f - deadzone));
                return deadzoneAdjusted;
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {

        }
    }
}