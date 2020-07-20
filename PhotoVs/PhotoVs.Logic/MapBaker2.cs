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

            var key = (tileset, material, sourceX, sourceY);
            var index = _sourceCache.FindIndex(s => s.Equals(key));
            if (index == -1)
            {
                _sourceCache.Add(key);
                index = _sourceCache.Count - 1;
            }

            m[(mapX, mapY, isMask)].Add(index);

            if (!_maxX.ContainsKey(mapName))
                _maxX.Add(mapName, 0);

            if (!_maxY.ContainsKey(mapName))
                _maxY.Add(mapName, 0);

            if (mapX > _maxX[mapName])
                _maxX[mapName] = mapX;

            if (mapY > _maxY[mapName])
                _maxY[mapName] = mapY;
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
            //Dictionary<(int, int, bool), List<int>>>

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
        }

        private void CreateSupertileset(bool isMaterial)
        {
            var rows = _dedupe.Count();
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

            var i = 0;
            foreach (var kvp in _dedupe)
            {
                var x = _tileSize * (i % _tilePerRow);
                var y = _tileSize * (i / _tilePerRow);

                foreach (var index in kvp.Key)
                {
                    var pos = _sourceCache[index];
                    _spriteBatch.Draw(isMaterial ? pos.Item2 : pos.Item1,
                    new Rectangle(x, y, _tileSize, _tileSize),
                    new Rectangle(pos.Item3, pos.Item4, _tileSize, _tileSize),
                    Color.White);
                }
                if (!_instructions.ContainsKey(kvp.Key))
                    _instructions.Add(kvp.Key, (x, y));

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
            var maxX = _maxX[mapName];
            var maxY = _maxY[mapName];

            maxX = FindNextPoT(maxX);
            maxY = FindNextPoT(maxY);

            var tm = _renderer.CreateRenderTarget(maxX * 2, maxY);
            _spriteBatch.GraphicsDevice.SetRenderTarget(tm);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();

            foreach (var t in tiles)
            {
                var (x, y, isMask) = t.Key;
                var sources = t.Value;
                var (sX , sY) = _instructions[sources];

                _spriteBatch.Draw(_pixelTexture,
                    new Vector2(x + (isMask ? 0 : maxX), y),
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
