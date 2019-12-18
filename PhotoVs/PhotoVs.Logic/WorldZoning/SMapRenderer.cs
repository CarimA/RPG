using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.Components;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Logic.Camera;
using PhotoVs.Models.Assets;
using PhotoVs.Models.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Logic.WorldZoning
{
    public class SMapRenderer : IDrawableSystem
    {
        private readonly IAssetLoader _assetLoader;
        private readonly SCamera _camera;
        private readonly SMapBoundaryGeneration _mapBoundary;

        private readonly Dictionary<string, string> _replaceCache;
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<string, Texture2D> _textureCache;
        private readonly Dictionary<int, ITileset> _tilesetCache;

        public SMapRenderer(SpriteBatch spriteBatch, IAssetLoader assetLoader, SMapBoundaryGeneration mapBoundary,
            SCamera camera)
        {
            _spriteBatch = spriteBatch;
            _assetLoader = assetLoader;
            _mapBoundary = mapBoundary;
            _camera = camera;

            _tilesetCache = new Dictionary<int, ITileset>();
            _textureCache = new Dictionary<string, Texture2D>();
            _replaceCache = new Dictionary<string, string>();
        }

        public int Priority { get; set; } = -99;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { };

        public void BeforeDraw(GameTime gameTime)
        {
            _camera.Attach(_spriteBatch);

            _mapBoundary.GetMaps().ForEach(DrawMapMask);

            _camera.Detach(_spriteBatch);
        }

        public void Draw(GameTime gameTime, IGameObjectCollection entities)
        {
        }

        public void AfterDraw(GameTime gameTime)
        {
            _camera.Attach(_spriteBatch);

            _mapBoundary.GetMaps().ForEach(DrawMapFringe);

            _camera.Detach(_spriteBatch);
        }

        private void DrawMapMask(Map map)
        {
            DrawMap(map, _spriteBatch, _assetLoader, "mask");
        }

        private void DrawMapFringe(Map map)
        {
            DrawMap(map, _spriteBatch, _assetLoader, "fringe");
        }

        private void DrawMap(Map map, SpriteBatch spriteBatch, IAssetLoader assetLoader, string layers)
        {
            foreach (var layer in map.Layers.Where(layer => layer.Name.Contains(layers)).OfType<TileLayer>())
                for (int y = 0, i = 0; y < layer.Height; y++)
                    for (var x = 0; x < layer.Width; x++, i++)
                    {
                        var gid = layer.Data[i];
                        if (gid == 0)
                            continue;

                        if (!_tilesetCache.TryGetValue(gid, out var tileset))
                        {
                            tileset = map.Tilesets.Single(ts =>
                                gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                            _tilesetCache.Add(gid, tileset);
                        }

                        var tile = tileset[gid];

                        if (!_replaceCache.TryGetValue(tileset.ImagePath, out var tilesetPath))
                        {
                            tilesetPath = tileset.ImagePath.Replace("../", "");
                            _replaceCache.Add(tileset.ImagePath, tilesetPath);
                        }

                        if (!_textureCache.TryGetValue(tilesetPath, out var tilesetTexture))
                        {
                            tilesetTexture = assetLoader.GetAsset<Texture2D>(tilesetPath);
                            _textureCache.Add(tilesetPath, assetLoader.GetAsset<Texture2D>(tilesetPath));
                        }

                        spriteBatch.Draw(tilesetTexture,
                            new Rectangle(map.XOffset + x * map.CellWidth, map.YOffset + y * map.CellHeight, map.CellWidth,
                                map.CellHeight),
                            new Rectangle(tile.Left, tile.Top, tile.Width, tile.Height),
                            Color.White);
                    }
        }
    }
}