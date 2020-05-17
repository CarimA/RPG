using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Desktop
{
    public class DesktopPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public string PaletteShader => "shaders/color.ogl";
        public IStreamProvider StreamProvider { get; set; }

        public DesktopPlatform()
        {
            StreamProvider = new FileSystemStreamProvider("assets/");
        }
    }
}
