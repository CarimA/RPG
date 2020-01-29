using System.IO;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.Assets;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class MapTypeLoader : ITypeLoader<Map>
    {
        public Map Load(Stream stream)
        {
            return Map.FromStream(stream);
        }
    }
}