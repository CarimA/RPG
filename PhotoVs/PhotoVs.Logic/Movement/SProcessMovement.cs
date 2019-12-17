using System;
using Microsoft.Xna.Framework;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Transforms;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Movement
{
    public class SProcessMovement : IUpdateableSystem
    {
        public int Priority { get; set; } = -2;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInput), typeof(CVelocity) };

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInput>();
                var velocity = entity.Components.Get<CVelocity>();

                var movement = input.Input.GetAxis();

                if (movement == Vector2.Zero)
                {
                    velocity.Velocity = Vector2.Zero;
                    break;
                }

                var isRunning = input.Input.ActionDown(InputActions.Run);

                // todo: make sense
                if (entity is Player player)
                {
                    movement *= player.CurrentSpeed(isRunning);
                    if (!player.CanMove())
                        movement *= 0;
                }
                else
                {
                    // important: do NOT multiply by deltatime
                    movement *= isRunning ? 350 : 200;
                }

                velocity.Velocity = movement;
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}