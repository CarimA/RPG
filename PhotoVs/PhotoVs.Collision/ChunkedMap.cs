using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Assets.AssetLoaders;
using PhotoVs.CommonGameLogic;
using PhotoVs.CommonGameLogic.Camera;
using PhotoVs.CommonGameLogic.Transforms;
using PhotoVs.Core.ECS.Entities;
using PhotoVs.DataStructures;
using PhotoVs.ECS.Components;
using PhotoVs.Extensions;
using PhotoVs.Math;
using PhotoVs.TiledMaps;
using PhotoVs.TiledMaps.Layers;
using PhotoVs.TiledMaps.Objects;

namespace PhotoVs.WorldZoning
{
    public class ChunkedMap
    {
        private readonly IAssetLoader _assetLoader;

        private readonly SpatialHash<Map> _chunks;
        private readonly SpatialHash<IEntity> _collisions;
        private readonly SpatialHash<IEntity> _scripts;
        private readonly SpatialHash<IEntity> _zones;

        public ChunkedMap(IAssetLoader assetLoader, string directory)
        {
            _assetLoader = assetLoader;

            _chunks = new SpatialHash<Map>(256);
            _collisions = new SpatialHash<IEntity>(256);
            _scripts = new SpatialHash<IEntity>(256);
            _zones = new SpatialHash<IEntity>(256);

            LoadMap(directory);
        }

        private void LoadMap(string directory)
        {
            _assetLoader
                .GetStreamProvider()
                .GetFiles(directory)
                .ForEach(LoadChunk);
        }

        private void LoadChunk(string file)
        {
            if (file.EndsWith("world"))
                return;

            var map = _assetLoader.GetAsset<Map>(file);
            var coords = Path.GetFileNameWithoutExtension(file)?.Split('_');

            if (coords != null &&
                int.TryParse(coords[0], out var mapX) &&
                int.TryParse(coords[1], out var mapY))
            {
                var bounds = new Rectangle(mapX * map.Width * map.CellWidth,
                    mapY * map.Height * map.CellHeight,
                    map.Width * map.CellWidth,
                    map.Height * map.CellHeight);
                map.XOffset = bounds.Left;
                map.YOffset = bounds.Top;
                _chunks.Add(map, bounds);

                ProcessLayers(map, mapX * map.Width * map.CellWidth, mapY * map.Height * map.CellHeight);
            }
            else
            {
                throw new InvalidDataException($"Chunk \"{file}\" has bad name");
            }

            _assetLoader.UnloadAsset(file);
        }

        private void ProcessLayers(Map map, int x, int y)
        {
            foreach (var layer in map.Layers)
            {
                if (!(layer is ObjectLayer objectLayer))
                    continue;

                foreach (var obj in objectLayer.Objects)
                {
                    if (layer.Name.Contains("col"))
                        ProcessObject(_collisions, obj, x, y, ProcessCollision);

                    if (obj.Properties.ContainsKey("script"))
                        ProcessObject(_scripts, obj, x, y, ProcessScript);

                    if (obj.Properties.ContainsKey("zone"))
                        ProcessObject(_zones, obj, x, y, ProcessZone);
                }
            }
        }

        private void ProcessObject(SpatialHash<IEntity> hash, BaseObject obj, int x, int y,
            Action<IEntity, BaseObject, int, int> func)
        {
            switch (obj)
            {
                case PolygonObject polygon:
                    ProcessPolygonObject(hash, polygon, x, y, func);
                    return;
                case RectangleObject rectangle:
                    ProcessRectangleObject(hash, rectangle, x, y, func);
                    return;
                default:
                    throw new NotImplementedException("Object Type not implemented");
            }
        }

        private void ProcessPolygonObject(SpatialHash<IEntity> hash, PolygonObject obj, int x, int y,
            Action<IEntity, BaseObject, int, int> func)
        {
            var entity = new Entity();
            var bounds = new CCollisionBound(obj.Polygon.Select(point => new Vector2(point.X, point.Y)).ToList());
            var position = new CPosition { Position = new Vector2(x + obj.X, y + obj.Y) };
            entity.Components.AddRange(new List<IComponent>
            {
                bounds,
                position
            });
            func(entity, obj, x, y);

            hash.Add(entity, new RectangleF(
                position.Position.X,
                position.Position.Y,
                bounds.Bounds.Width,
                bounds.Bounds.Height
            ));
        }

        private void ProcessRectangleObject(SpatialHash<IEntity> hash, RectangleObject obj, int x, int y,
            Action<IEntity, BaseObject, int, int> func)
        {
            var entity = new Entity();
            var bounds = CCollisionBound.Rectangle(new Vector2(obj.Width, obj.Height));
            var position = new CPosition { Position = new Vector2(x + obj.X, y + obj.Y) };
            entity.Components.AddRange(new List<IComponent>
            {
                bounds,
                position
            });

            func(entity, obj, x, y);

            hash.Add(entity, new RectangleF(
                position.Position.X,
                position.Position.Y,
                bounds.Bounds.Width,
                bounds.Bounds.Height
            ));
        }

        private void ProcessCollision(IEntity entity, BaseObject obj, int x, int y)
        {
            entity.Components.Add(new CSolid());
        }

        private void ProcessScript(IEntity entity, BaseObject obj, int x, int y)
        {
            entity.Components.Add(new CScript(obj.Properties["script"]));
        }

        private void ProcessZone(IEntity entity, BaseObject obj, int x, int y)
        {
            entity.Components.Add(new CZone(obj.Properties["zone"]));
        }


        public (List<Map>, List<IEntity>, List<IEntity>, List<IEntity>) GetDataInBounds(Rectangle bounds)
        {
            return (_chunks.Get(bounds),
                _collisions.Get(bounds),
                _scripts.Get(bounds),
                _zones.Get(bounds));
        }

        public (List<Map>, List<IEntity>, List<IEntity>, List<IEntity>) GetDataInCamera(SCamera camera)
        {
            var bounds = camera.VisibleArea();
            /*bounds.X = bounds.X / ChunkWidthInPixels - 1;
            bounds.Y = bounds.Y / ChunkHeightInPixels - 1;
            bounds.Width = bounds.Width / ChunkWidthInPixels + 3;
            bounds.Height = bounds.Height / ChunkHeightInPixels + 3;*/
            return GetDataInBounds(bounds);
        }
    }
}
