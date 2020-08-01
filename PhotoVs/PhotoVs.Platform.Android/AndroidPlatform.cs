using System.Collections.Generic;
using Android.Content.Res;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Audio;

namespace PhotoVs.Platform.Android
{
    public class AndroidPlatform : IPlatform
    {
        public AndroidPlatform(AssetManager assetManager)
        {
            StreamProvider = new AndroidStreamProvider(assetManager);
            FileExtensionReplacement = new Dictionary<string, string>
            {
                {".fx", ".ogl"}
            };
            Audio = new DummyAudio();
        }

        public bool OverrideFullscreen => true;
        public Dictionary<string, string> FileExtensionReplacement { get; }
        public IAudio Audio { get; }
        public IStreamProvider StreamProvider { get; set; }
    }
}