using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Models.ECS;
using System;

namespace PhotoVs.Logic.Input
{
    public class SHandleFullscreen : IUpdateableSystem
    {
        public int Priority { get; set; } = -999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInput) };

        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;

        private int _windowWidth;
        private int _windowHeight;

        public SHandleFullscreen(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
        {
            _graphics = graphics;
            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInput>().Input;
                if (!input.ActionPressed(InputActions.Fullscreen))
                    continue;
                if (_graphics.IsFullScreen)
                {
                    DisableFullscreen();
                }
                else
                {
                    EnableFullscreen();
                }
            }
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

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}
