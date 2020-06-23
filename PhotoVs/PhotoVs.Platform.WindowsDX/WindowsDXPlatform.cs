using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Logic;
using System;
using System.IO;

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
