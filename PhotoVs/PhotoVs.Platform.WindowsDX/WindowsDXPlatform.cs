using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Logic;

namespace PhotoVs.Platform.WindowsDX
{
    public class WindowsDXPlatform : IPlatform
    {
        public string PaletteShader => "shaders/color.dx11";
        public IStreamProvider StreamProvider { get; set; }

        public WindowsDXPlatform()
        {
            StreamProvider = new FileSystemStreamProvider("assets/");
        }
    }
}
