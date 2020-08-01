using System.IO;
using PhotoVs.Engine.Assets.AssetLoaders;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class TextTypeLoader : TypeLoader<string>
    {
        public TextTypeLoader(IAssetLoader assetLoader) : base(assetLoader)
        {
        }

        public override string Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}