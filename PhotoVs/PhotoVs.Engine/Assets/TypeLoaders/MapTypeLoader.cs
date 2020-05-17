using PhotoVs.Engine.TiledMaps;
using System.IO;

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