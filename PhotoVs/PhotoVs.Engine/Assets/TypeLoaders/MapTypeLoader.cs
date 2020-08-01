using System.IO;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.TiledMaps;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class MapTypeLoader : TypeLoader<Map>
    {
        public MapTypeLoader(IAssetLoader assetLoader) : base(assetLoader)
        {
        }

        public override Map Load(Stream stream)
        {
            return Map.FromStream(stream);
        }
    }
}