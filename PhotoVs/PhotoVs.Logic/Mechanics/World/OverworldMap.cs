using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics.Particles;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Engine.TiledMaps.Objects;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World.Components;
using PhotoVs.Logic.Particles;
using PhotoVs.Utils;
using PhotoVs.Utils.Collections;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.World
{
    public class OverworldMap
    {
        private readonly int _cellHeight;

        private readonly int _cellWidth;
        private readonly IAssetLoader _assetLoader;
        private readonly Quadtree<GameObject> _collisions;
        private readonly Quadtree<GameObject> _scripts;
        private readonly Quadtree<GameObject> _zones;
        private readonly Quadtree<IEmitter> _maskEmitters;
        private readonly Quadtree<IEmitter> _fringeEmitters;

        // todo: hold map texture in here
        public OverworldMap(Map map, IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;

            Properties = map.Properties;
            _cellWidth = map.CellWidth;
            _cellHeight = map.CellHeight;

            var mapWidth = map.Width * map.CellWidth;
            var mapHeight = map.Height * map.CellHeight;
            var rect = new RectangleF(0, 0, mapWidth, mapHeight);

            var chunkSize = 256;

            _collisions = new Quadtree<GameObject>(rect);
            _scripts = new Quadtree<GameObject>(rect);
            _zones = new Quadtree<GameObject>(rect);
            _maskEmitters = new Quadtree<IEmitter>(rect);
            _fringeEmitters = new Quadtree<IEmitter>(rect);

            ParseTiles(map);
            ParseObjects(map);
        }

        public Dictionary<string, string> Properties { get; }

        private void ParseTiles(Map map)
        {
            var isMask = true;
            var tilesetCache = new Dictionary<int, ITileset>();

            map.Layers
                .OfType<TileLayer>()
                .ForEach(layer =>
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

                        var posX = map.XOffset + x * map.CellWidth + layer.X;
                        var posY = map.YOffset + y * map.CellHeight + layer.Y;

                        /*if (tileset.Name == "trees_fringe")
                        {
                            _emitters.Add(new Emitter<Leaf>(30,
                                    _assetLoader.Get<Texture2D>("particles/leaf.png"),
                                    new Rectangle(posX, posY, 30, 30)),
                                new RectangleF(posX, posY, 30, 30));
                        }*/

                        var relativeId = gid - tileset.FirstGid;
                        if (tileset.TileProperties.TryGetValue(relativeId, out var item))
                        {
                            var values = item.Values.Select(i => i);
                            foreach (var value in values)
                            {
                                var split = value.Split('|');
                                    switch (split[0])
                                    {
                                        case "emitter":
                                            var px = (int)float.Parse(split[2]);
                                            var py = (int)float.Parse(split[3]);
                                            var width = (int)float.Parse(split[4]);
                                            var height = (int)float.Parse(split[5]);
                                            var bounds = new Rectangle(tile.Left + px, tile.Top + py, width, height);

                                            if (!new Rectangle(tile.Left, tile.Top, tile.Width, tile.Height)
                                                .Contains(bounds))
                                                continue;

                                            var ty = posY - (tile.Height) + 16 + py;
                                            var boundsT = new Rectangle(posX + px, ty, width, height);
                                            var boundsF = new RectangleF(posX + px, ty, width, height);

                                            switch (split[1])
                                            {
                                                case "leaf":
                                                    _maskEmitters.Add(new Emitter<Leaf>(5,
                                                        _assetLoader.Get<Texture2D>("particles/leaf.png"),
                                                        boundsT), boundsF);

                                                    _fringeEmitters.Add(new Emitter<Leaf>(2,
                                                        _assetLoader.Get<Texture2D>("particles/leaf.png"),
                                                        boundsT), boundsF);
                                                    break;
                                                default:
                                                    break;
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                        }
                    }
                });
        }

        private void ParseObjects(Map map)
        {
            map.Layers
                .OfType<ObjectLayer>()
                .ForEach(layer =>
                {
                    layer.Objects
                        .ForEach(mapObject =>
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

        private void ProcessObject(Quadtree<GameObject> hash, BaseObject obj,
            Action<GameObject, BaseObject> func)
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

        private void ProcessPolygonObject(Quadtree<GameObject> hash, PolygonObject obj,
            Action<GameObject, BaseObject> func)
        {
            var entity = new GameObject();
            var bounds = new CCollisionBound(obj.Polygon.Select(point => new Vector2(point.X, point.Y)).ToList());
            var position = new CPosition(new Vector2(obj.X, obj.Y));
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

        private void ProcessRectangleObject(Quadtree<GameObject> hash, RectangleObject obj,
            Action<GameObject, BaseObject> func)
        {
            var entity = new GameObject();
            var bounds = CCollisionBound.Rectangle(new Vector2(obj.Width, obj.Height));
            var position = new CPosition(new Vector2(obj.X, obj.Y));

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

        private void ProcessCollision(GameObject entity, BaseObject obj)
        {
            entity.Components.Add(new CSolid(true));
        }

        private void ProcessScript(GameObject entity, BaseObject obj)
        {
            entity.Components.Add(new CScript(obj.Properties["script"]));
        }

        private void ProcessZone(GameObject entity, BaseObject obj)
        {
            entity.Components.Add(new CZone(obj.Properties["zone"]));
        }

        public IEnumerable<GameObject> GetCollisions(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _collisions.Find(camera.VisibleArea());
        }

        public IEnumerable<GameObject> GetScripts(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _scripts.Find(camera.VisibleArea());
        }

        public IEnumerable<GameObject> GetZones(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _zones.Find(camera.VisibleArea());
        }

        public IEnumerable<IEmitter> GetMaskEmitters(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _maskEmitters.Find(camera.VisibleArea());
        }

        public IEnumerable<IEmitter> GetFringeEmitters(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _fringeEmitters.Find(camera.VisibleArea());
        }
    }
}