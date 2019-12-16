using System.IO;
using PhotoVs.TiledMaps;

namespace PhotoVs.Assets.TypeLoaders
{
    public class MapTypeLoader : ITypeLoader<Map>
    {
        public Map Load(Stream stream)
        {
            return Map.FromStream(stream);
        }
    }
}