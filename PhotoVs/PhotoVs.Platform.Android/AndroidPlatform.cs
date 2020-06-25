using Android.Content.Res;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Platform.Android
{
    public class AndroidPlatform : IPlatform
    {
        public bool OverrideFullscreen => true;
        public string ShaderFileExtension => ".ogl";
        public IStreamProvider StreamProvider { get; set; }

        public AndroidPlatform(AssetManager assetManager)
        {
            StreamProvider = new AndroidStreamProvider(assetManager);
        }
    }
}
