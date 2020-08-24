using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.World
{
    public class Overworld : IOverworld
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Random _random;

        private readonly Dictionary<string, OverworldMap> _maps;
        private readonly SpriteBatch _spriteBatch;
        private OverworldMap _currentMap;

        public Overworld(SpriteBatch spriteBatch, IAssetLoader assetLoader, Random random)
        {
            _spriteBatch = spriteBatch;
            _assetLoader = assetLoader;
            _random = random;

            _maps = new Dictionary<string, OverworldMap>();
        }

        public void LoadMaps(string directory)
        {
            var files = _assetLoader
                .StreamProvider
                .EnumerateFiles(DataLocation.Content, directory);
            files.ForEach(LoadMap);
        }

        public void SetMap(string map)
        {
            _currentMap = _maps[map];
        }

        public OverworldMap GetMap()
        {
            return _currentMap;
        }

        private void LoadMap(string filepath)
        {
            if (!filepath.EndsWith("tmx"))
                return;

            var name = filepath.Replace('\\', '/');
            if (name.Contains("/")) name = name.Substring(name.LastIndexOf('/') + 1);

            name = name.Substring(0, name.Length - ".tmx".Length);

            _maps[name] = new OverworldMap(_random, _assetLoader.Get<Map>(filepath), _assetLoader);
        }
    }
}