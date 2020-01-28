﻿using Microsoft.Xna.Framework;

namespace PhotoVs.Models.FSM
{
    public interface IDrawableScene : IScene
    {
        void Draw(GameTime gameTime);
        void DrawUI(GameTime gameTime, Matrix uiOrigin);
    }
}