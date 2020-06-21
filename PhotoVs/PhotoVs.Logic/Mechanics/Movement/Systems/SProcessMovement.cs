using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.PlayerData;
using System;
using PhotoVs.Logic.Mechanics.Input.Components;

namespace PhotoVs.Logic.Mechanics.Movement.Systems
{
    public class SProcessMovement : IUpdateableSystem
    {
        public int Priority { get; set; } = -2;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInputState), typeof(CPosition) };

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInputState>();
                var position = entity.Components.Get<CPosition>();

                var movement = input.LeftAxis;

                if (movement == Vector2.Zero)
                {
                    // this looks redundant but it's actually to indicate that nothing changed
                    // so the Position setter can run and set LastPosition
                    position.Position = position.Position;
                    break;
                }

                var isRunning = input.ActionDown(InputActions.Run);

                // todo: make sense
                if (entity is Player player)
                {
                    movement *= player.CurrentSpeed(isRunning);
                    if (!player.CanMove)
                        movement *= 0;
                }
                else
                {
                    // important: do NOT multiply by deltatime
                    movement *= isRunning ? 350 : 200;
                }

                position.Position += movement * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}