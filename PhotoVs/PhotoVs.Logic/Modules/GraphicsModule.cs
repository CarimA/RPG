using System;
using PhotoVs.Engine;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scripting;

namespace PhotoVs.Logic.Modules
{
    public class GraphicsModule : Module
    {
        private readonly Renderer _renderer;

        public GraphicsModule(Services services)
        {
            _renderer = services.Get<Renderer>();
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
            _renderer.ToggleFullscreen();
        }
    }
}
