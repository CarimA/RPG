using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics
{
    public class Animation
    {
        [System(RunOn.Update, typeof(CAnimation))]
        public void UpdateAnimations(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var animation = gameObject.Components.Get<CAnimation>();
                animation.Update(gameTime);
            }
        }

        // todo: refactor elsewhere
        [System(RunOn.Update, typeof(CAnimation), typeof(CPosition), typeof(CAlive))]
        public void UpdateDirection(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var animation = gameObject.Components.Get<CAnimation>();
                var position = gameObject.Components.Get<CPosition>();
                var running = gameObject.Components.Has<CRunning>();

                if (position.DeltaPosition == Vector2.Zero)
                {
                    switch (position.Direction)
                    {
                        case Direction.Up:
                            animation.Play("idle-up");
                            break;
                        case Direction.Down:
                            animation.Play("idle-down");
                            break;
                        case Direction.Left:
                            animation.Play("idle-left");
                            break;
                        case Direction.Right:
                            animation.Play("idle-right");
                            break;
                    }
                }
                else
                {
                    if (running)
                    {
                        switch (position.Direction)
                        {
                            case Direction.Up:
                                animation.Play("run-up");
                                break;
                            case Direction.Down:
                                animation.Play("run-down");
                                break;
                            case Direction.Left:
                                animation.Play("run-left");
                                break;
                            case Direction.Right:
                                animation.Play("run-right");
                                break;
                        }
                    }
                    else
                    {
                        switch (position.Direction)
                        {
                            case Direction.Up:
                                animation.Play("walk-up");
                                break;
                            case Direction.Down:
                                animation.Play("walk-down");
                                break;
                            case Direction.Left:
                                animation.Play("walk-left");
                                break;
                            case Direction.Right:
                                animation.Play("walk-right");
                                break;
                        }
                    }
                }
            }
        }
    }
}
