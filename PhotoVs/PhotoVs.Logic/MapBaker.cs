using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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
        private Dictionary<(int, int, string), bool> _transparencyCache;
        private Dictionary<(int, int, string), bool> _opaqueCache;

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
            _transparencyCache = new Dictionary<(int, int, string), bool>();
            _opaqueCache = new Dictionary<(int, int, string), bool>();
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
            var i = 0;
            foreach (var map in maps)
            {
                if (map.Layers.OfType<TileLayer>()
                    .All(layer => layer.Name.StartsWith("M") || layer.Name.StartsWith("F")))
                {
                    yield return map;
                    continue;
                }

                TmxMap.CompressLayers(map, _assetLoader);

                i++;
                using var filestream = new FileStream(Path.Combine(_outputDir, $"temp-{i}.tmx"), FileMode.Create);
                using var writer = new XmlTextWriter(filestream, Encoding.UTF8);
                writer.WriteWholeMap(map);

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
                for (var x = 0; x < layer.Width; x++, i++)
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

                    if (tileset.TileWidth > _tileSize || tileset.TileHeight > _tileSize)
                    {
                        var xCarry = 0;
                        for (var xx = tile.Left; xx < tile.Left + tileset.TileWidth; xx += _tileSize)
                        {
                            var yCarry = 0;
                            for (var yy = tile.Top + tileset.TileHeight - _tileSize; yy > tile.Top; yy -= _tileSize)
                            {
                                if (_transparencyCache.TryGetValue((xx, yy, tilesetPath), out var value))
                                {
                                    if (value)
                                        continue;
                                }
                                else
                                {
                                    var colorData = GetTextureData(tilesetTexture,
                                        new Rectangle(xx, yy, _tileSize, _tileSize));
                                    var result = colorData.All(color => color == Color.Transparent);
                                    _transparencyCache.Add((xx, yy, tilesetPath), result);
                                    if (result)
                                        continue;
                                }

                                if (!_opaqueCache.TryGetValue((xx, yy, tilesetPath), out var opaque))
                                {
                                    var colorData = GetTextureData(tilesetTexture,
                                        new Rectangle(xx, yy, _tileSize, _tileSize));
                                    opaque = colorData.All(color => color != Color.Transparent);
                                    _opaqueCache.Add((xx, yy, tilesetPath), opaque);
                                }

                                if (!_mapCache.ContainsKey((x + xCarry, y - yCarry, isMask, mapName)))
                                    _mapCache.Add((x + xCarry, y - yCarry, isMask, mapName), new KeyList<(int, int, string, Texture2D, Texture2D)>());

                                if (opaque)
                                {
                                    _mapCache[(x + xCarry, y - yCarry, isMask, mapName)].Clear();
                                }

                                _mapCache[(x + xCarry, y - yCarry, isMask, mapName)].Add((xx, yy, mapName, tilesetTexture, materialTexture));
                                yCarry++;
                            }
                            xCarry++;
                        }

                        if (!_maxX.ContainsKey(mapName))
                            _maxX.Add(mapName, 0);

                        if (!_maxY.ContainsKey(mapName))
                            _maxY.Add(mapName, 0);

                        if ((x + xCarry) > _maxX[mapName])
                            _maxX[mapName] = x + xCarry;

                        if ((y) > _maxY[mapName])
                            _maxY[mapName] = y;
                    }
                    else
                    {
                        var colorData = GetTextureData(tilesetTexture,
                            new Rectangle(tile.Left, tile.Top, tile.Width, tile.Height));
                        if (colorData.All(color => color == Color.Transparent))
                        {
                            continue;
                        }

                        if (!_opaqueCache.TryGetValue((tile.Left, tile.Top, tilesetPath), out var opaque))
                        {
                            var colorDat2a = GetTextureData(tilesetTexture,
                                new Rectangle(tile.Left, tile.Top, _tileSize, _tileSize));
                            opaque = colorDat2a.All(color => color != Color.Transparent);
                            _opaqueCache.Add((tile.Left, tile.Top, tilesetPath), opaque);
                        }

                        if (!_mapCache.ContainsKey((x, y, isMask, mapName)))
                            _mapCache.Add((x, y, isMask, mapName), new KeyList<(int, int, string, Texture2D, Texture2D)>());


                        if (opaque)
                        {
                            _mapCache[(x, y, isMask, mapName)].Clear();
                        }

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
        }

        private void DeduplicateIndexes()
        {
            var copy = _mapCache.Where(k => k.Value.Count > 0);
            foreach (var kvp in copy)
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


        private static Color[] GetTextureData(Texture2D texture, Rectangle rect)
        {
            var imageData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(imageData);

            return GetImageData(imageData, texture.Width, rect);
        }

        private static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            var color = new Color[rectangle.Width * rectangle.Height];
            for (var x = 0; x < rectangle.Width; x++)
            for (var y = 0; y < rectangle.Height; y++)
                color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
            return color;
        }
    }
}
