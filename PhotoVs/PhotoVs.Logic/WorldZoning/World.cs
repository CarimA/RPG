using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Models.Assets;
using PhotoVs.Utils.Extensions;
using System.Collections.Generic;

namespace PhotoVs.Logic.WorldZoning
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
            _assetLoader
                .GetStreamProvider()
                .GetDirectories(directory)
                .ForEach(LoadMap);
        }

        private void LoadMap(string directory)
        {
            _maps[directory] = new ChunkedMap(_assetLoader, directory);
        }

        public ChunkedMap GetMap(string map)
        {
            return _maps[map];
        }
    }
}