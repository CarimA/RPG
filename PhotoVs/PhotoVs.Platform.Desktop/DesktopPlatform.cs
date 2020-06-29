using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;
using System;
using System.IO;

namespace PhotoVs.Platform.Desktop
{
    public class DesktopPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public string ShaderFileExtension => ".ogl";
        public IStreamProvider StreamProvider { get; set; }

        public DesktopPlatform()
        {
            StreamProvider = new FileSystemStreamProvider(
                "content\\",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs"));
        }
    }
}
