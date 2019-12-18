using PhotoVs.Engine.TiledMaps;
using PhotoVs.Models.Assets;
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