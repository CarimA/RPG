using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;
using System;
using System.IO;

namespace PhotoVs.Platform.WindowsDX
{
    public class WindowsDXPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public string ShaderFileExtension => ".dx11";
        public IStreamProvider StreamProvider { get; set; }

        public WindowsDXPlatform()
        {
            StreamProvider = new FileSystemStreamProvider(
                "content\\",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs"));
        }
    }
}
