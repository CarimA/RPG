using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Logic;

namespace PhotoVs.Platform.WindowsDX
{
    public class WindowsDXPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public string PaletteShader => "shaders/color.dx11";
        public IStreamProvider StreamProvider { get; set; }

        public WindowsDXPlatform()
        {
            StreamProvider = new FileSystemStreamProvider("assets/");
        }
    }
}
