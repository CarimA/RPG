using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Models.ECS;
using System;
using System.IO;

namespace PhotoVs.Logic.Input
{
    public class STakeScreenshot : IUpdateableSystem
    {
        private GraphicsDevice _graphicsDevice;

        public int Priority { get; set; } = 999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CInput) };

        public STakeScreenshot(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInput>().Input;
                if (input.ActionPressed(InputActions.Screenshot))
                {
                    TakeScreenshot();
                }
            }
        }

        private void TakeScreenshot()
        {
            var width = _graphicsDevice.PresentationParameters.BackBufferWidth;
            var height = _graphicsDevice.PresentationParameters.BackBufferHeight;
            var data = new Color[width * height];
            _graphicsDevice.GetBackBufferData(data);
            using var texture = new Texture2D(_graphicsDevice, width, height);
            texture.SetData(data);
            using var stream = File.Create($"screenshots/{Guid.NewGuid().ToString()}.png");
            texture.SaveAsPng(stream, width, height);
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}
