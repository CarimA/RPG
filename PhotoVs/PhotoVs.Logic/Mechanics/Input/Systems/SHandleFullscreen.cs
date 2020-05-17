﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Input.Components;
using System;

namespace PhotoVs.Logic.Mechanics.Input.Systems
{
    public class SHandleFullscreen : IUpdateableSystem
    {
        // todo: move this logic out of a system, return to MainGame

        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private int _windowHeight;

        private int _windowWidth;

        public SHandleFullscreen(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
        {
            _graphics = graphics;
            _graphicsDevice = graphicsDevice;
        }

        public int Priority { get; set; } = -999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInput) };

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInput>().Input;
                if (!input.ActionPressed(InputActions.Fullscreen))
                    continue;
                if (_graphics.IsFullScreen)
                    DisableFullscreen();
                else
                    EnableFullscreen();
            }
        }

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void DisableFullscreen()
        {
            _graphics.PreferredBackBufferWidth = _windowWidth;
            _graphics.PreferredBackBufferHeight = _windowHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        private void EnableFullscreen()
        {
            _windowWidth = _graphicsDevice.PresentationParameters.Bounds.Width;
            _windowHeight = _graphicsDevice.PresentationParameters.Bounds.Height;

            _graphics.PreferredBackBufferWidth = _graphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }
    }
}