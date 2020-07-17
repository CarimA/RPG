using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;
using System;
using System.Collections.Generic;
using System.IO;
using PhotoVs.Engine.Audio;

namespace PhotoVs.Platform.Desktop
{
    public class DesktopPlatform : IPlatform
    {
        public bool OverrideFullscreen => false;
        public Dictionary<string, string> FileExtensionReplacement { get; }
        public IAudio Audio { get; }
        public IStreamProvider StreamProvider { get; set; }

        public DesktopPlatform()
        {
            StreamProvider = new FileSystemStreamProvider(
                "content\\",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs"));
            FileExtensionReplacement = new Dictionary<string, string>
            {
                {".fx", ".ogl"}
            };
            Audio = new DummyAudio();
        }
    }
}
