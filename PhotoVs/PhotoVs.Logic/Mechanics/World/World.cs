using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Utils.Extensions;
using System.Collections.Generic;

namespace PhotoVs.Logic.Mechanics.World
{
    public class World
    {
        private readonly IAssetLoader _assetLoader;

        private readonly Dictionary<string, ChunkedMap> _maps;
        private readonly SpriteBatch _spriteBatch;

        public World(SpriteBatch spriteBatch, IAssetLoader assetLoader)
        {
            _spriteBatch = spriteBatch;
            _assetLoader = assetLoader;

            _maps = new Dictionary<string, ChunkedMap>();
        }

        public void LoadMaps(string directory)
        {
            var directories = _assetLoader
                .GetStreamProvider()
                .GetDirectories(directory);
            directories.ForEach(LoadMap);
        }

        private void LoadMap(string directory)
        {
            var name = directory.Replace('\\', '/');
            if (name.Contains("/"))
            {
                name = name.Substring(name.LastIndexOf('/') + 1);
            }
            _maps[name] = new ChunkedMap(_assetLoader, directory);
        }

        public ChunkedMap GetMap(string map)
        {
            var name = map.Replace('\\', '/');
            if (name.Contains("/"))
            {
                name = name.Substring(name.LastIndexOf('/') + 1);
            }
            return _maps[name];
        }
    }
}