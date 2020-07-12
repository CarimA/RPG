using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;

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

        public MapBaker(IAssetLoader assetLoader, SpriteBatch spriteBatch, Renderer renderer)
        {
            _assetLoader = assetLoader;
            _spriteBatch = spriteBatch;
            _renderer = renderer;
        }

        private int FindNextPoT(int input)
        {
            var power = 1;
            while (power < input)
                power *= 2;
            return power;
        }

        // todo: take a list of tmx maps
        // todo: cache every tileset texture and use accordingly
        // todo: refactor this into separate functions as an actual class
        public void BakeMap(string inputFile, string outputMapTexture, string outputTilesetTexture)
        {
            var maxOutputWidth = 4096;
            var maxOutputHeight = 4096;
            var tileSize = 16;

            var map = _assetLoader.Get<Map>(inputFile);
            var tilesetCache = new Dictionary<int, ITileset>();
            var mapCache = new Dictionary<(int, int, bool), KeyList<(int, int)>>();

            var pixelTexture = _assetLoader.Get<Texture2D>("ui/pixel.png");
            var tilesetTexture = _assetLoader.Get<Texture2D>("tilesets/tileset_nature.png");

            var maskLayers = map.Layers.OfType<TileLayer>().Where(layer => layer.Name.StartsWith("M")); // .TakeWhile(layer => !layer.Name.Equals("FringeStart"));
            var fringeLayers = map.Layers.OfType<TileLayer>().Where(layer => layer.Name.StartsWith("F")); //.SkipWhile(layer => !layer.Name.Equals("FringeStart"));

            var maxX = 0;
            var maxY = 0;

            // first process the entire map and assign a list of tiles
            // to each position
            foreach (var tileLayer in maskLayers)
            {
                for (int y = 0, i = 0; y < tileLayer.Height; y++)
                {
                    for (int x = 0; x < tileLayer.Width; x++, i++)
                    {
                        var gid = tileLayer.Data[i];
                        if (gid == 0)
                            continue;

                        if (!tilesetCache.TryGetValue(gid, out var tileset))
                        {
                            tileset = map.Tilesets.Single(ts =>
                                gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                            tilesetCache.Add(gid, tileset);
                        }

                        var tile = tileset[gid];

                        if (!mapCache.ContainsKey((x, y, false)))
                            mapCache.Add((x, y, false), new KeyList<(int, int)>());

                        mapCache[(x, y, false)].Add((tile.Left, tile.Top));

                        if (x > maxX)
                            maxX = x;

                        if (y > maxY)
                            maxY = y;
                    }
                }
            }

            foreach (var tileLayer in fringeLayers)
            {
                for (int y = 0, i = 0; y < tileLayer.Height; y++)
                {
                    for (int x = 0; x < tileLayer.Width; x++, i++)
                    {
                        var gid = tileLayer.Data[i];
                        if (gid == 0)
                            continue;

                        if (!tilesetCache.TryGetValue(gid, out var tileset))
                        {
                            tileset = map.Tilesets.Single(ts =>
                                gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                            tilesetCache.Add(gid, tileset);
                        }

                        var tile = tileset[gid];

                        if (!mapCache.ContainsKey((x, y, true)))
                            mapCache.Add((x, y, true), new KeyList<(int, int)>());

                        mapCache[(x, y, true)].Add((tile.Left, tile.Top));

                        if (x > maxX)
                            maxX = x;

                        if (y > maxY)
                            maxY = y;
                    }
                }
            }

            // next de-duplicate the keylists into indexes that can point to
            // the same tile
            var keyCache = new List<KeyList<(int, int)>>();
            var mapIndexes = new Dictionary<(int, int, bool), int>();
            foreach (var kvp in mapCache)
            {
                var index = keyCache.FindIndex(key => key.Equals(kvp.Value));
                if (index >= 0)
                {
                    mapIndexes.Add(kvp.Key, index);
                }
                else
                {
                    keyCache.Add(kvp.Value);
                    mapIndexes.Add(kvp.Key, keyCache.Count - 1);
                }
            }

            var rows = keyCache.Count();

            // create a supertileset in the size we need
            var outputHeight = Math.Min(FindNextPoT(rows / tileSize), maxOutputHeight / tileSize);
            var outputWidth = FindNextPoT((rows / outputHeight));

            if (outputWidth > maxOutputWidth)
                throw new Exception("Too many tiles! How tf did you manage to do that?");

            outputHeight *= tileSize;
            outputWidth *= tileSize;

            var tilePerRow = outputWidth / tileSize;

            // now render the big tileset
            var ts = _renderer.CreateRenderTarget(outputWidth, outputHeight);
            _spriteBatch.GraphicsDevice.SetRenderTarget(ts);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();
            for (var i = 0; i < keyCache.Count; i++)
            {
                var x = tileSize * (i % tilePerRow);
                var y = tileSize * (i / tilePerRow);
                var t = keyCache[i];

                foreach (var ti in t)
                {
                    _spriteBatch.Draw(tilesetTexture,
                        new Rectangle(x, y, tileSize, tileSize),
                        new Rectangle(ti.Item1, ti.Item2, tileSize, tileSize),
                        Color.White);
                }
            }

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            using var stream = File.Create(outputTilesetTexture);
            {
                ts.SaveAsPng(stream, ts.Width, ts.Height);
            }

            // now render the tilemap thing
            maxX = FindNextPoT(maxX);
            maxY = FindNextPoT(maxY);

            var tm = _renderer.CreateRenderTarget(maxX * 2, maxY);
            _spriteBatch.GraphicsDevice.SetRenderTarget(tm);
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();

            foreach (var kvp in mapIndexes)
            {
                var x = kvp.Key.Item1;
                var y = kvp.Key.Item2;
                var isMask = kvp.Key.Item3;
                var sX = kvp.Value % tilePerRow;
                var sY = kvp.Value / tilePerRow;

                _spriteBatch.Draw(pixelTexture,
                    new Vector2(x + (!isMask ? 0 : maxX), y), 
                    new Color(sX, sY, 0));
            }

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            using var stream2 = File.Create(outputMapTexture);
            {
                tm.SaveAsPng(stream2, tm.Width, tm.Height);
            }
        }

    }
}
