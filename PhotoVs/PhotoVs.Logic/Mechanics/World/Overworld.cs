using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Utils.Extensions;
using System.Collections.Generic;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.TiledMaps;

namespace PhotoVs.Logic.Mechanics.World
{
    public class Overworld
    {
        private readonly IAssetLoader _assetLoader;

        private readonly Dictionary<string, OverworldMap> _maps;
        private OverworldMap _currentMap;
        private readonly SpriteBatch _spriteBatch;

        public Overworld(SpriteBatch spriteBatch, IAssetLoader assetLoader)
        {
            _spriteBatch = spriteBatch;
            _assetLoader = assetLoader;

            _maps = new Dictionary<string, OverworldMap>();
        }

        public void LoadMaps(string directory)
        {
            var files = _assetLoader
                .StreamProvider
                .EnumerateFiles(DataLocation.Content, directory);
            files.ForEach(LoadMap);
        }

        private void LoadMap(string filepath)
        {
            var name = filepath.Replace('\\', '/');
            if (name.Contains("/"))
            {
                name = name.Substring(name.LastIndexOf('/') + 1);
            }

            name = name.Substring(0, name.Length - ".tmx".Length);

            _maps[name] = new OverworldMap(_assetLoader.Get<Map>(filepath), _assetLoader);
        }

        public void SetMap(string map)
        {
            _currentMap = _maps[map];
        }

        public OverworldMap GetMap()
        {
            return _currentMap;
        }
    }
}