﻿using System;
using Microsoft.Xna.Framework;
using PhotoVs.Logic.Transforms;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Movement
{
    public class SProcessVelocity : IUpdateableSystem
    {
        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = {typeof(CPosition), typeof(CVelocity)};

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var position = entity.Components.Get<CPosition>();
                var velocity = entity.Components.Get<CVelocity>().Velocity;

                position.Position += velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}