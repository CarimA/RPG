using System;
using System.Collections.Generic;
using System.IO;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Audio;

namespace PhotoVs.Platform.WindowsDX
{
    public class WindowsDXPlatform : IPlatform
    {
        public WindowsDXPlatform()
        {
            StreamProvider = new FileSystemStreamProvider(
                "content\\",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs"));
            FileExtensionReplacement = new Dictionary<string, string>
            {
                {".fx", ".dx11"}
            };
            Audio = new CscoreAudio();
        }

        public bool OverrideFullscreen => false;
        public Dictionary<string, string> FileExtensionReplacement { get; }
        public IAudio Audio { get; }
        public IStreamProvider StreamProvider { get; set; }
    }
}