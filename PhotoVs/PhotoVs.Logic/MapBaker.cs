using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Utils.Extensions;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace PhotoVs.Logic
{

    sealed class KeyList<T> : List<T>, IEquatable<KeyList<T>>, IEnumerable
    {
        int _hashCode;
        List<T> _items;

        public KeyList()
        {
            _items = new List<T>();
            _hashCode = 0;
        }

        public KeyList(int capacity)
        {
            _items = new List<T>(capacity);
            _hashCode = 0;
        }

        public T this[int index] => _items[index];

        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _items.Add(item);
            if (null != item)
                _hashCode ^= item.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(KeyList<T> rhs)
        {
            if (ReferenceEquals(this, rhs))
                return true;
            if (ReferenceEquals(rhs, null))
                return false;
            if (rhs._hashCode != _hashCode)
                return false;
            var ic = _items.Count;
            if (ic != rhs._items.Count)
                return false;
            for (var i = 0; i < ic; ++i)
                if (!Equals(_items[i], rhs._items[i]))
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyList<T>);
        }
    }

    class MapBaker
    {
        private IAssetLoader _assetLoader;
        private SpriteBatch _spriteBatch;
        private Renderer _renderer;
        private Texture2D _pixelTexture;

        private const int _maxOutputWidth = 4096;
        private const int _maxOutputHeight = 4096;
        private const int _tileSize = 16;

        private int _tilePerRow;
        private Dictionary<string, int> _maxX;
        private Dictionary<string, int> _maxY;
        private Dictionary<string, Texture2D> _textureCache;
        private Dictionary<string, Texture2D> _materialCache;
        private Dictionary<string, string> _replaceCache;
        private Dictionary<(int, int, bool, string), KeyList<(int, int, string, Texture2D, Texture2D)>> _mapCache;
        private Dictionary<(int, int, bool, string), int> _mapIndexes;
        private List<KeyList<(int, int, string, Texture2D, Texture2D)>> _keyCache;

        private string _outputDir;

        public MapBaker(IAssetLoader assetLoader, SpriteBatch spriteBatch, Renderer renderer)
        {
            _assetLoader = assetLoader;
            _spriteBatch = spriteBatch;
            _renderer = renderer;

            _pixelTexture = _assetLoader.Get<Texture2D>("ui/pixel.png");

            _maxX = new Dictionary<string, int>();
            _maxY = new Dictionary<string, int>();
            _textureCache = new Dictionary<string, Texture2D>();
            _materialCache = new Dictionary<string, Texture2D>();
            _replaceCache = new Dictionary<string, string>();
            _mapCache = new Dictionary<(int, int, bool, string), KeyList<(int, int, string, Texture2D, Texture2D)>>();
            _mapIndexes = new Dictionary<(int, int, bool, string), int>();
            _keyCache = new List<KeyList<(int, int, string, Texture2D, Texture2D)>>();
        }

        private int FindNextPoT(int input)
        {
            var power = 1;
            while (power < input)
                power *= 2;
            return power;
        }


        public void Bake(string inputDir, string outputDir)
        {
            _outputDir = outputDir;

            var maps = LoadMaps(inputDir);
            var compressedMaps = CompressMaps(maps);
            ProcessTileIndexes(compressedMaps);
            DeduplicateIndexes();
            CreateSupertileset(false);
            CreateSupertileset(true);
            CreateTilemaps();
        }

        private IEnumerable<Map> LoadMaps(string inputDir)
        {
            var streamProvider = _assetLoader.StreamProvider;
            var maps = streamProvider.EnumerateFiles(DataLocation.Raw, inputDir);
            foreach (var map in maps)
            {
                using var obj = streamProvider.Read(DataLocation.Raw, map);
                var data = _assetLoader.Process<Map>(obj);
                yield return data;
            }
        }

        private IEnumerable<Map> CompressMaps(IEnumerable<Map> maps)
        {
            foreach (var map in maps)
            {
                if (map.Layers.OfType<TileLayer>()
                    .All(layer => layer.Name.StartsWith("M") || layer.Name.StartsWith("F")))
                {
                    yield return map;
                    continue;
                }

                TmxMap.CompressLayers(map, _assetLoader);
                yield return map;
            }
        }

        private void ProcessTileIndexes(IEnumerable<Map> maps)
        {
            foreach (var map in maps)
            {
                ProcessTileIndex(map);
            }
        }

        private void ProcessTileIndex(Map map)
        {
            var maskLayers = map.Layers.OfType<TileLayer>().Where(layer => layer.Name.StartsWith("M"));
            var fringeLayers = map.Layers.OfType<TileLayer>().Where(layer => layer.Name.StartsWith("F"));

            ProcessLayers(map, maskLayers, true);
            ProcessLayers(map, fringeLayers, false);
        }

        private void ProcessLayers(Map map, IEnumerable<TileLayer> layers, bool isMask)
        {
            foreach (var layer in layers)
            {
                ProcessLayer(map, layer, isMask);
            }
        }

        private void ProcessLayer(Map map, TileLayer layer, bool isMask)
        {
            for (int y = 0, i = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++, i++)
                {
                    var gid = layer.Data[i];
                    if (gid == 0)
                        continue;

                    var tileset = map.Tilesets.Single(ts =>
                            gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                    var tile = tileset[gid];

                    if (!_replaceCache.TryGetValue(tileset.ImagePath, out var tilesetPath))
                    {
                        tilesetPath = tileset.ImagePath.Replace("../", "");
                        tilesetPath = "tilesets/" + Path.GetFileName(tilesetPath);
                        _replaceCache.Add(tileset.ImagePath, tilesetPath);
                    }

                    if (!_textureCache.TryGetValue(tilesetPath, out var tilesetTexture))
                    {
                        tilesetTexture = _assetLoader.Get<Texture2D>(tilesetPath);
                        _textureCache.Add(tilesetPath, _assetLoader.Get<Texture2D>(tilesetPath));
                    }

                    var materialPath = tilesetPath.Substring(0, tilesetPath.Length - ".png".Length) +
                                       "_mat.png";
                    if (!_materialCache.TryGetValue(materialPath, out var materialTexture))
                    {
                        materialTexture = _assetLoader.Get<Texture2D>(materialPath);
                        _materialCache.Add(materialPath, _assetLoader.Get<Texture2D>(materialPath));
                    }

                    var mapName = map.Properties["name"];

                    if (!_mapCache.ContainsKey((x, y, isMask, mapName)))
                        _mapCache.Add((x, y, isMask, mapName), new KeyList<(int, int, string, Texture2D, Texture2D)>());

                    _mapCache[(x, y, isMask, mapName)].Add((tile.Left, tile.Top, mapName, tilesetTexture, materialTexture));

                    if (!_maxX.ContainsKey(mapName))
                        _maxX.Add(mapName, 0);

                    if (!_maxY.ContainsKey(mapName))
                        _maxY.Add(mapName, 0);

                    if (x > _maxX[mapName])
                        _maxX[mapName] = x;

                    if (y > _maxY[mapName])
                        _maxY[mapName] = y;
                }
            }
        }

        private void DeduplicateIndexes()
        {
            foreach (var kvp in _mapCache)
            {
                var index = _keyCache.FindIndex(key =>
                {
                    var count = 0;
                    foreach (var a in key)
                    {
                        foreach (var b in kvp.Value)
                        {
                            if (a.Item1 == b.Item1 && a.Item2 == b.Item2 && a.Item4 == b.Item4)
                                count++;
                        }
                    }

                    if (count == key.Count && count == kvp.Value.Count)
                        return true;
                    else
                        return false;
                });

                if (index >= 0)
                {
                    _mapIndexes.Add(kvp.Key, index);
                }
                else
                {
                    _keyCache.Add(kvp.Value);
                    _mapIndexes.Add(kvp.Key, _keyCache.Count - 1);
                }
            }
        }

        private void CreateSupertileset(bool isMaterial)
        {
            // create a supertileset in the size we need
            var rows = _keyCache.Count();
            var outputHeight = Math.Min(FindNextPoT(rows / _tileSize), _maxOutputHeight / _tileSize);
            var outputWidth = FindNextPoT((rows / outputHeight));

            if (outputWidth > _maxOutputWidth)
                throw new Exception("Too many tiles! How tf did you manage to do that?");

            outputHeight *= _tileSize;
            outputWidth *= _tileSize;

            _tilePerRow = outputWidth / _tileSize;

            // now render the big tileset
            var ts = _renderer.CreateRenderTarget(outputWidth, outputHeight);
            _spriteBatch.GraphicsDevice.SetRenderTarget(ts);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();
            for (var i = 0; i < _keyCache.Count; i++)
            {
                var x = _tileSize * (i % _tilePerRow);
                var y = _tileSize * (i / _tilePerRow);
                var t = _keyCache[i];

                foreach (var (xpos, ypos, mapName, texture, material) in t)
                {
                    _spriteBatch.Draw(isMaterial ? material : texture,
                        new Rectangle(x, y, _tileSize, _tileSize),
                        new Rectangle(xpos, ypos, _tileSize, _tileSize),
                        Color.White);
                }
            }

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            var outName = isMaterial ? "supertileset_mat.png" : "supertileset.png";
            ts.SaveAsPng(Path.Combine(_outputDir, outName));
        }

        private void CreateTilemaps()
        {
            // figure out all of the maps from the keycache
            var mapNames = GetMapNames();
            mapNames = mapNames.Distinct();

            foreach (var map in mapNames)
            {
                CreateTilemap(map);
            }
        }

        private void CreateTilemap(string mapName)
        {
            var maxX = _maxX[mapName];
            var maxY = _maxY[mapName];

            maxX = FindNextPoT(maxX);
            maxY = FindNextPoT(maxY);

            var tm = _renderer.CreateRenderTarget(maxX * 2, maxY);
            _spriteBatch.GraphicsDevice.SetRenderTarget(tm);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();

            foreach (var kvp in _mapIndexes)
            {
                var mN = kvp.Key.Item4;
                if (!mapName.Equals(mN))
                    continue;

                var x = kvp.Key.Item1;
                var y = kvp.Key.Item2;
                var isMask = kvp.Key.Item3;
                var sX = kvp.Value % _tilePerRow;
                var sY = kvp.Value / _tilePerRow;

                _spriteBatch.Draw(_pixelTexture,
                    new Vector2(x + (isMask ? 0 : maxX), y),
                    new Color(sX, sY, 0));
            }

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            var outName = $"maps/{mapName}.png";
            tm.SaveAsPng(Path.Combine(_outputDir, outName));
            tm.Dispose();
        }

        private IEnumerable<string> GetMapNames()
        {
            foreach (var key in _keyCache)
            {
                foreach (var item in key)
                {
                    yield return item.Item3;
                }
            }
        }
    }
}
