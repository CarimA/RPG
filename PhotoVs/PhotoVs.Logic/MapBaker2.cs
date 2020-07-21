using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic
{
    class MapBaker2
    {
        private const int _maxOutputWidth = 4096;
        private const int _maxOutputHeight = 4096;

        private readonly string _inputDir;
        private readonly string _outputDir;
        private readonly int _tileSize;

        private readonly IAssetLoader _assetLoader;
        private readonly SpriteBatch _spriteBatch;
        private readonly Renderer _renderer;

        private readonly Texture2D _pixelTexture;

        private readonly IEnumerable<Map> _mapData;
        private readonly Dictionary<string, Texture2D> _textureCache;
        private readonly Dictionary<string, Texture2D> _materialCache;

        private readonly Dictionary<Map, Dictionary<(int, int, bool), KeyList<int>>> _maps;
        private readonly Dictionary<KeyList<int>, List<(Map, int, int, bool)>> _dedupe;

        private readonly List<(Texture2D, Texture2D, int, int)> _sourceCache;
        private readonly Dictionary<(Texture2D, int, int), bool> _transparencyCache;
        private readonly Dictionary<(Texture2D, int, int), bool> _opaqueCache;

        private readonly Dictionary<Texture2D, Color[]> _textureDataCache;

        private readonly Dictionary<KeyList<int>, (int, int)> _instructions;

        private Dictionary<string, int> _minX;
        private Dictionary<string, int> _minY;
        private Dictionary<string, int> _maxX;
        private Dictionary<string, int> _maxY;

        private int _tilePerRow;

        public MapBaker2(Services services, string inputDir, string outputDir, int tileSize)
        {
            _assetLoader = services.Get<IAssetLoader>();
            _spriteBatch = services.Get<SpriteBatch>();
            _renderer = services.Get<Renderer>();

            _pixelTexture = _assetLoader.Get<Texture2D>("ui/pixel.png");

            _inputDir = inputDir;
            _outputDir = outputDir;
            _tileSize = tileSize;

            var mapData = LoadMaps(inputDir);
            //var compressedMapData = CompressMaps(mapData);
            _mapData = mapData; //compressedMapData;

            _maps = new Dictionary<Map, Dictionary<(int, int, bool), KeyList<int>>>();
            _dedupe = new Dictionary<KeyList<int>, List<(Map, int, int, bool)>>();

            _sourceCache = new List<(Texture2D, Texture2D, int, int)>();
            _textureCache = new Dictionary<string, Texture2D>();
            _materialCache = new Dictionary<string, Texture2D>();
            _transparencyCache = new Dictionary<(Texture2D,  int, int), bool>();
            _opaqueCache = new Dictionary<(Texture2D, int, int), bool>();
            _maxX = new Dictionary<string, int>();
            _maxY = new Dictionary<string, int>();
            _minX = new Dictionary<string, int>();
            _minY = new Dictionary<string, int>();
            _textureDataCache = new Dictionary<Texture2D, Color[]>();
            _instructions = new Dictionary<KeyList<int>, (int, int)>();
        }

        public void Bake()
        {
            ProcessMaps();
            Deduplicate();
            CreateSupertileset(false);
            CreateSupertileset(true);
            CreateTilemaps();
        }

        private void ProcessMaps()
        {
            foreach (var map in _mapData) 
                ProcessMap(map);
        }

        private void ProcessMap(Map map)
        {
            var layers = map.Layers.OfType<TileLayer>();
            var maskLayers = layers.TakeWhile(layer => !layer.Name.Equals("FringeStart"));
            var fringeLayers = layers.SkipWhile(layer => !layer.Name.Equals("FringeStart"));


            //var maskLayers = map.Layers.OfType<TileLayer>().Where(layer => layer.Name.StartsWith("M"));
            //var fringeLayers = map.Layers.OfType<TileLayer>().Where(layer => layer.Name.StartsWith("F"));

            ProcessLayers(map, maskLayers, true);
            ProcessLayers(map, fringeLayers, false);
        }

        private void ProcessLayers(Map map, IEnumerable<TileLayer> layers, bool isMask)
        {
            foreach (var layer in layers)
                ProcessLayer(map, layer, isMask);
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

                    var tilesetPath = tileset.ImagePath.Replace("../", "");
                    tilesetPath = "tilesets/" + Path.GetFileName(tilesetPath);

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
                        ProcessOversizedTile(map,
                            tilesetTexture,
                            materialTexture,
                            mapName,
                            tile.Left,
                            tile.Top,
                            tileset.TileWidth,
                            tileset.TileHeight,
                            x,
                            y,
                            isMask);
                    }
                    else
                    {
                        ProcessTile(
                            map,
                            tilesetTexture, 
                            materialTexture,
                            mapName,
                            tile.Left,
                            tile.Top,
                            x,
                            y,
                            isMask);
                    }
                }
            }
        }

        private void ProcessOversizedTile(Map map, Texture2D tileset, Texture2D material, string mapName, int sourceX, int sourceY, int tileWidth, int tileHeight, int mapX, int mapY, bool isMask)
        {
            var xi = 0;
            for (var x = sourceX; x < sourceX + tileWidth; x += _tileSize)
            {
                var yi = -(tileHeight / _tileSize);
                for (var y = sourceY; y < sourceY + tileHeight; y += _tileSize)
                {
                    ProcessTile(map,
                        tileset,
                        material,
                        mapName,
                        x,
                        y,
                            mapX + xi,
                        mapY + yi + 1,
                        isMask);
                    yi++;
                }

                xi++;
            }
        }

        private void ProcessTile(Map map, Texture2D tileset, Texture2D material, string mapName, int sourceX, int sourceY, int mapX, int mapY, bool isMask)
        {
            // if transparent, just ignore it entirely
            if (CheckIfTransparent((tileset, sourceX, sourceY)))
                return;

            if (!_maps.ContainsKey(map))
                _maps.Add(map, new Dictionary<(int, int, bool), KeyList<int>>());

            var m = _maps[map];
            
            if (!m.ContainsKey((mapX, mapY, isMask)))
                m.Add((mapX, mapY, isMask), new KeyList<int>());

            // if opaque, clear out everything underneath this tile
            if (CheckIfOpaque((tileset, sourceX, sourceY)))
            {
                m[(mapX, mapY, isMask)] = new KeyList<int>();

                if (!isMask)
                {
                    m.Remove((mapX, mapY, true));
                }
            }
            else
            {
                // check if it occludes everything underneath it
                var tileData = GetTextureData(tileset, new Rectangle(sourceX, sourceY, _tileSize, _tileSize));
                var isOccluded = true;

                // we look at only the transparent segments and see if anything underneath has a pixel there.
                // if it's also 100% transparent in the same spots
                foreach (var i in m[(mapX, mapY, isMask)])
                {
                    var source = _sourceCache[i];
                    var testData = GetTextureData(source.Item1,
                        new Rectangle(source.Item3, source.Item4, _tileSize, _tileSize));

                    for (var c = 0; c < tileData.Length; c++)
                    {
                        var main = tileData[c];
                        var test = testData[c];

                        if (main.A == byte.MaxValue)
                            continue;

                        if (test.A == byte.MinValue)
                            continue;

                        isOccluded = false;
                        break;
                    }

                    if (!isOccluded)
                        break;
                }

                if (isOccluded)
                {
                    m[(mapX, mapY, isMask)] = new KeyList<int>();
                }
            }

            var key = (tileset, material, sourceX, sourceY);
            var index = _sourceCache.FindIndex(s => s.Equals(key));
            if (index == -1)
            {
                _sourceCache.Add(key);
                index = _sourceCache.Count - 1;
            }

            m[(mapX, mapY, isMask)].Add(index);

            if (!_maxX.ContainsKey(mapName))
                _maxX.Add(mapName, int.MinValue);

            if (!_maxY.ContainsKey(mapName))
                _maxY.Add(mapName, int.MinValue);

            if (mapX > _maxX[mapName])
                _maxX[mapName] = mapX;

            if (mapY > _maxY[mapName])
                _maxY[mapName] = mapY;


            if (!_minX.ContainsKey(mapName))
                _minX.Add(mapName, int.MaxValue);

            if (!_minY.ContainsKey(mapName))
                _minY.Add(mapName, int.MaxValue);

            if (mapX < _minX[mapName])
                _minX[mapName] = mapX;

            if (mapY < _minY[mapName])
                _minY[mapName] = mapY;
        }

        private bool CheckIfTransparent((Texture2D, int, int) index)
        {
            if (!_transparencyCache.TryGetValue(index, out var value))
            {
                var colorData = GetTextureData(index.Item1, new Rectangle(index.Item2, index.Item3, _tileSize, _tileSize));
                var isTransparent = colorData.All(color => color == Color.Transparent);
                value = isTransparent;
                _transparencyCache.Add(index, value);
            }
            return value;
        }

        private bool CheckIfOpaque((Texture2D, int, int) index)
        {
            if (!_opaqueCache.TryGetValue(index, out var value))
            {
                var colorData = GetTextureData(index.Item1, new Rectangle(index.Item2, index.Item3, _tileSize, _tileSize));
                var isOpaque = colorData.All(color => color.A == byte.MaxValue);
                value = isOpaque;
                _opaqueCache.Add(index, value);
            }
            return value;
        }

        private void Deduplicate()
        {
            var toUpdate = new List<(Map, (int, int, bool), KeyList<int>)>();

            // run through maps and convert into a usable format
            foreach (var kvp in _maps)
            {
                var map = kvp.Key;
                var tileData = kvp.Value;

                foreach (var td in tileData)
                {
                    var tileInfo = td.Key;
                    var list = td.Value;

                    if (!_dedupe.ContainsKey(list))
                        _dedupe.Add(list, new List<(Map, int, int, bool)>());

                    _dedupe[list].Add((map, tileInfo.Item1, tileInfo.Item2, tileInfo.Item3));
                }
            }

            // get combination of dedupe list
            /*var combinations = new Dictionary<int, (KeyValuePair<KeyList<int>, List<(Map, int, int, bool)>>,
                KeyValuePair<KeyList<int>, List<(Map, int, int, bool)>>)>();
            var removedCount = 0;

            foreach (var a in _dedupe)
            {
                foreach (var b in _dedupe)
                {
                    if (a.Equals(b))
                        continue;

                    var key = (a, b);

                    if (combinations.ContainsKey(key.GetHashCode()))
                        continue;

                    if (combinations.ContainsKey((b, a).GetHashCode()))
                        continue;

                    combinations.Add(key.GetHashCode(), key);
                }
            }

            // run through the combinations and compare stuff
            foreach (var kvp in combinations)
            {
                var a = kvp.Value.Item1;
                var b = kvp.Value.Item2;

                // check that both are still actually present in dedupe
                if (!_dedupe.ContainsKey(a.Key) || !_dedupe.ContainsKey(b.Key))
                    continue;
                
                var tileA = Enumerable.Repeat(Color.Transparent, _tileSize * _tileSize).ToArray();
                var tileB = Enumerable.Repeat(Color.Transparent, _tileSize * _tileSize).ToArray();

                foreach (var i in a.Key)
                {
                    var ai = _sourceCache[i];
                    var ac = GetTextureData(ai.Item1,
                        new Rectangle(ai.Item3, ai.Item4, _tileSize, _tileSize));

                    for (var p = 0; p < tileA.Length; p++)
                    {
                        if (ac[p] != Color.Transparent)
                            tileA[p] = ac[p];
                    }
                }

                foreach (var i in b.Key)
                {
                    var bi = _sourceCache[i];
                    var bc = GetTextureData(bi.Item1,
                        new Rectangle(bi.Item3, bi.Item4, _tileSize, _tileSize));

                    for (var q = 0; q < tileB.Length; q++)
                    {
                        if (bc[q] != Color.Transparent)
                            tileB[q] = bc[q];
                    }
                }

                if (tileA.SequenceEqual(tileB) || a.Equals(b))
                {
                    // it's a duplicate! remove b from both _dedupe and _instructions and update _maps

                    foreach (var kvp2 in _maps)
                    {
                        foreach (var kvp3 in kvp2.Value)
                        {
                            if (kvp3.Value.SequenceEqual(b.Key))
                                toUpdate.Add((kvp2.Key, kvp3.Key, a.Key));
                        }
                    }

                    _dedupe[a.Key].AddRange(_dedupe[b.Key]);
                    _dedupe.Remove(b.Key);
                    removedCount++;
                }

            }

            foreach (var u in toUpdate)
            {
                if (_dedupe.ContainsKey(u.Item3))
                    _maps[u.Item1][u.Item2] = u.Item3;
            }

            Console.WriteLine($"Removed {removedCount} duplicates.");*/
        }

        private void CreateSupertileset(bool isMaterial)
        {
            var tiles = _dedupe.Count;
            var outputWidth = 1;
            var outputHeight = 1;
            var alternator = false;

            while ((outputWidth * outputHeight) < tiles)
            {
                if (alternator)
                    outputWidth *= 2;
                else
                    outputHeight *= 2;

                alternator = !alternator;
            }

            outputWidth *= _tileSize;
            outputHeight *= _tileSize;

            if (outputWidth > _maxOutputWidth || outputHeight > _maxOutputHeight)
                throw new Exception("Too many tiles! How tf did you manage to do that?");

            _tilePerRow = outputWidth / _tileSize;

            // now render the big tileset
            var ts = _renderer.CreateRenderTarget(outputWidth, outputHeight);
            _spriteBatch.GraphicsDevice.SetRenderTarget(ts);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            var copy = _dedupe.Keys.ToList();
            copy = copy.Distinct().ToList();

            var i = 0;
            foreach (var key in copy)
            {
                var x = _tileSize * (i % _tilePerRow);
                var y = _tileSize * (i / _tilePerRow);

                foreach (var index in key)
                {
                    var pos = _sourceCache[index];
                    _spriteBatch.Draw(isMaterial ? pos.Item2 : pos.Item1,
                    new Rectangle(x, y, _tileSize, _tileSize),
                    new Rectangle(pos.Item3, pos.Item4, _tileSize, _tileSize),
                    Color.White);
                }
                if (!_instructions.ContainsKey(key))
                    _instructions.Add(key, (x, y));

                i++;
            }

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            var outName = isMaterial ? "supertileset_mat.png" : "supertileset.png";
            ts.SaveAsPng(Path.Combine(_outputDir, outName));
        }

        private void CreateTilemaps()
        {
            foreach (var kvp in _maps)
            {
                CreateTilemap(kvp.Key, kvp.Value);
            }
        }

        private void CreateTilemap(Map map, Dictionary<(int, int, bool), KeyList<int>> tiles)
        {
            var mapName = map.Properties["name"];
            var minX = _minX[mapName];
            var minY = _minY[mapName];
            var maxX = _maxX[mapName];
            var maxY = _maxY[mapName];

            var sizeX = FindNextPoT(maxX - minX);
            var sizeY = FindNextPoT(maxY - minY);

            var tm = _renderer.CreateRenderTarget(sizeX * 2, sizeY);
            _spriteBatch.GraphicsDevice.SetRenderTarget(tm);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();

            foreach (var t in tiles)
            {
                var (x, y, isMask) = t.Key;
                var sources = t.Value;

                var (sX, sY) = _instructions[sources];

                _spriteBatch.Draw(_pixelTexture,
                    new Vector2(x + (isMask ? 0 : sizeX) - minX, y - minY),
                    new Color(sX / _tileSize, sY / _tileSize, 0));
            }

            _spriteBatch.End();
            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            var outName = $"maps/{mapName}.png";
            tm.SaveAsPng(Path.Combine(_outputDir, outName));
            tm.Dispose();
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

        private static int FindNextPoT(int input)
        {
            var power = 1;
            while (power < input)
                power *= 2;
            return power;
        }

        private Color[] GetTextureData(Texture2D texture, Rectangle rect)
        {
            if (!_textureDataCache.TryGetValue(texture, out var value))
            {
                value = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(value);
                _textureDataCache.Add(texture, value);
            }

            return GetImageData(value, texture.Width, rect);
        }

        private Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            var color = new Color[rectangle.Width * rectangle.Height];
            for (var x = 0; x < rectangle.Width; x++)
            for (var y = 0; y < rectangle.Height; y++)
                color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
            return color;
        }
    }
}
