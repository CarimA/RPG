﻿using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.ECS
{
    public interface IUpdateableSystem : ISystem
    {
        void BeforeUpdate(GameTime gameTime);
        void Update(GameTime gameTime, IGameObjectCollection entities);
        void AfterUpdate(GameTime gameTime);
    }
}