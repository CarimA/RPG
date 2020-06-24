using PhotoVs.Engine.Assets.StreamProviders;
using System;
using System.IO;
using PhotoVs.Engine;

namespace PhotoVs.Platform.Desktop
{
    public class DesktopPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public string PaletteShader => "shaders/color.ogl";
        public string AverageShader => "shaders/average.ogl";
        public IStreamProvider StreamProvider { get; set; }

        public DesktopPlatform()
        {
            StreamProvider = new FileSystemStreamProvider(
                "content\\",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs"));
        }
    }
}
