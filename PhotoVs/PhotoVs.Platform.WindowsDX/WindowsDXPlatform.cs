using PhotoVs.Engine.Assets.StreamProviders;
using System;
using System.IO;
using PhotoVs.Engine;

namespace PhotoVs.Platform.WindowsDX
{
    public class WindowsDXPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public string PaletteShader => "shaders/color.dx11";
        public string AverageShader => "shaders/average.dx11";
        public IStreamProvider StreamProvider { get; set; }

        public WindowsDXPlatform()
        {
            StreamProvider = new FileSystemStreamProvider(
                "content\\",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs"));
        }
    }
}
