using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Engine.TiledMaps.Objects;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Utils;
using PhotoVs.Utils.Collections;
using PhotoVs.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoVs.Logic.Mechanics.World
{
    public class OverworldMap
    {
        private IAssetLoader _assetLoader;

        public Dictionary<string, string> Properties { get; }

        private SpatialHash<Tile> _maskTiles;
        private SpatialHash<Tile> _fringeTiles;
        private SpatialHash<IGameObject, GameObjectCollection> _collisions;
        private SpatialHash<IGameObject, GameObjectCollection> _scripts;
        private SpatialHash<IGameObject, GameObjectCollection> _zones;

        private readonly int _cellWidth;
        private readonly int _cellHeight;

        public OverworldMap(Map map, IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;

            Properties = map.Properties;
            _cellWidth = map.CellWidth;
            _cellHeight = map.CellHeight;

            var chunkSize = 256;
            _maskTiles = new SpatialHash<Tile>(chunkSize);
            _fringeTiles = new SpatialHash<Tile>(chunkSize);
            _collisions = new SpatialHash<IGameObject, GameObjectCollection>(chunkSize);
            _scripts = new SpatialHash<IGameObject, GameObjectCollection>(chunkSize);
            _zones = new SpatialHash<IGameObject, GameObjectCollection>(chunkSize);

            ParseTiles(map);
            ParseObjects(map);
        }

        private void ParseTiles(Map map)
        {
            var isMask = true;
            var tilesetCache = new Dictionary<int, ITileset>();
            var textureCache = new Dictionary<string, Texture2D>();
            var replaceCache = new Dictionary<string, string>();

            map.Layers
                .OfType<TileLayer>()
                .ForEach((layer) =>
                {
                    if (layer.Name.Equals("FringeStart"))
                        isMask = false;

                    for (int y = 0, i = 0; y < layer.Height; y++)
                        for (var x = 0; x < layer.Width; x++, i++)
                        {
                            var gid = layer.Data[i];
                            if (gid == 0)
                                continue;

                            if (!tilesetCache.TryGetValue(gid, out var tileset))
                            {
                                tileset = map.Tilesets.Single(ts =>
                                    gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                                tilesetCache.Add(gid, tileset);
                            }

                            var tile = tileset[gid];

                            if (!replaceCache.TryGetValue(tileset.ImagePath, out var tilesetPath))
                            {
                                tilesetPath = tileset.ImagePath.Replace("../", "");
                                tilesetPath = "tilesets/" + Path.GetFileName(tilesetPath);
                                replaceCache.Add(tileset.ImagePath, tilesetPath);
                            }

                            if (!textureCache.TryGetValue(tilesetPath, out var tilesetTexture))
                            {
                                tilesetTexture = _assetLoader.Get<Texture2D>(tilesetPath);
                                textureCache.Add(tilesetPath, _assetLoader.Get<Texture2D>(tilesetPath));
                            }

                            var posX = map.XOffset + (x * map.CellWidth) + layer.X;
                            var posY = map.YOffset + (y * map.CellHeight) + layer.Y;

                            var tileData = new Tile(posX, posY, tile.Left, tile.Top, tilesetTexture);

                            if (isMask)
                            {
                                _maskTiles.Add(tileData, new RectangleF(posX, posY, tile.Width, tile.Height));
                            }
                            else
                            {
                                _fringeTiles.Add(tileData, new Rectangle(posX, posY, tile.Width, tile.Height));
                            }
                        }
                });
        }

        private void ParseObjects(Map map)
        {
            map.Layers
                .OfType<ObjectLayer>()
                .ForEach((layer) =>
                {
                    layer.Objects
                        .ForEach((mapObject) =>
                        {
                            if (layer.Name.Contains("col"))
                                ProcessObject(_collisions, mapObject, ProcessCollision);

                            if (mapObject.Properties.ContainsKey("script"))
                                ProcessObject(_scripts, mapObject, ProcessScript);

                            if (mapObject.Properties.ContainsKey("zone"))
                                ProcessObject(_zones, mapObject, ProcessZone);
                        });
                });
        }

        private void ProcessObject(SpatialHash<IGameObject, GameObjectCollection> hash, BaseObject obj,
            Action<IGameObject, BaseObject> func)
        {
            switch (obj)
            {
                case PolygonObject polygon:
                    ProcessPolygonObject(hash, polygon, func);
                    return;
                case RectangleObject rectangle:
                    ProcessRectangleObject(hash, rectangle, func);
                    return;
                default:
                    throw new NotImplementedException("Object Type not implemented");
            }
        }

        private void ProcessPolygonObject(SpatialHash<IGameObject, GameObjectCollection> hash, PolygonObject obj,
            Action<IGameObject, BaseObject> func)
        {
            var entity = new GameObject();
            var bounds = new CCollisionBound(obj.Polygon.Select(point => new Vector2(point.X, point.Y)).ToList());
            var position = new CPosition { Position = new Vector2(obj.X, obj.Y) };
            entity.Components.Add(bounds);
            entity.Components.Add(position);

            func(entity, obj);

            hash.Add(entity, new RectangleF(
                position.Position.X,
                position.Position.Y,
                bounds.Bounds.Width,
                bounds.Bounds.Height
            ));
        }

        private void ProcessRectangleObject(SpatialHash<IGameObject, GameObjectCollection> hash, RectangleObject obj,
            Action<IGameObject, BaseObject> func)
        {
            var entity = new GameObject();
            var bounds = CCollisionBound.Rectangle(new Vector2(obj.Width, obj.Height));
            var position = new CPosition { Position = new Vector2(obj.X, obj.Y) };

            entity.Components.Add(bounds);
            entity.Components.Add(position);

            func(entity, obj);

            hash.Add(entity, new RectangleF(
                position.Position.X,
                position.Position.Y,
                bounds.Bounds.Width,
                bounds.Bounds.Height
            ));
        }

        private void ProcessCollision(IGameObject entity, BaseObject obj)
        {
            entity.Components.Add(new CSolid());
        }

        private void ProcessScript(IGameObject entity, BaseObject obj)
        {
            entity.Components.Add(new CScript(obj.Properties["script"]));
        }

        private void ProcessZone(IGameObject entity, BaseObject obj)
        {
            entity.Components.Add(new CZone(obj.Properties["zone"]));
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, SCamera camera, Action<SpriteBatch, GameTime> drawBetween)
        {
            //var (maskTiles, fringeTiles) = GetTilesInBoundary(camera.VisibleArea());
            var boundaries = camera.VisibleArea();

            DrawLayer(_maskTiles, boundaries, spriteBatch, gameTime);
            drawBetween(spriteBatch, gameTime);
            DrawLayer(_fringeTiles, boundaries, spriteBatch, gameTime);

            //System.Diagnostics.Debug.WriteLine("Drew " + maskTiles.Count + " masks");
            //System.Diagnostics.Debug.WriteLine("Drew " + fringeTiles.Count + " fringes");
        }

        private (IEnumerable<Tile>, IEnumerable<Tile>) GetTilesInBoundary(Rectangle boundaries)
        {
            return (_maskTiles.Get(boundaries), _fringeTiles.Get(boundaries));
        }

        private void DrawLayer(SpatialHash<Tile> tiles, Rectangle boundaries, SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var tile in tiles.Get(boundaries))
            {
                spriteBatch.Draw(tile.Texture,
                    new Rectangle(tile.X, tile.Y, _cellWidth, _cellHeight),
                    new Rectangle(tile.SourceX, tile.SourceY, _cellWidth, _cellHeight),
                    Color.White);
            }
        }


        public IEnumerable<IGameObject> GetCollisions(SCamera camera)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            return _collisions.Get(camera.VisibleArea());
        }

        public IEnumerable<IGameObject> GetScripts(SCamera camera)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            return _scripts.Get(camera.VisibleArea());
        }

        public IEnumerable<IGameObject> GetZones(SCamera camera)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            return _zones.Get(camera.VisibleArea());
        }
    }
}
