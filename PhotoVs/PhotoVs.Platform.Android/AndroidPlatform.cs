using System.Collections.Generic;
using Android.Content.Res;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Platform.Android
{
    public class AndroidPlatform : IPlatform
    {
        public bool OverrideFullscreen => true;
        public Dictionary<string, string> FileExtensionReplacement { get; }
        public IStreamProvider StreamProvider { get; set; }

        public AndroidPlatform(AssetManager assetManager)
        {
            StreamProvider = new AndroidStreamProvider(assetManager);
            FileExtensionReplacement = new Dictionary<string, string>
            {
                {".fx", ".ogl"}
            };
        }
    }
}
