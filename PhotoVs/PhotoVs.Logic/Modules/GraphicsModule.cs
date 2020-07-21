using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Scripting;

namespace PhotoVs.Logic.Modules
{
    public class GraphicsModule : Module
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private int _windowHeight;
        private int _windowWidth;

        public GraphicsModule(Services services)
        {
            _graphicsDevice = services.Get<GraphicsDevice>();
            _graphics = services.Get<GraphicsDeviceManager>();
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("ToggleFullscreen", (Action)ToggleFullscreen);

            base.DefineApi(interpreter);
        }

        private void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
            {
                DisableFullscreen();
            }
            else
            {
                EnableFullscreen();
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

    }
}
