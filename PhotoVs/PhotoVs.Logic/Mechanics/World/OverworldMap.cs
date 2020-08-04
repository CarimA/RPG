using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Engine.TiledMaps.Objects;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.World.Components;
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
        private readonly SpatialHash<GameObject, GameObjectList> _collisions;
        private readonly SpatialHash<GameObject, GameObjectList> _scripts;
        private readonly SpatialHash<GameObject, GameObjectList> _zones;

        // todo: hold map texture in here
        public OverworldMap(Map map, IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;

            Properties = map.Properties;
            _cellWidth = map.CellWidth;
            _cellHeight = map.CellHeight;

            var chunkSize = 256;
            _collisions = new SpatialHash<GameObject, GameObjectList>(chunkSize);
            _scripts = new SpatialHash<GameObject, GameObjectList>(chunkSize);
            _zones = new SpatialHash<GameObject, GameObjectList>(chunkSize);

            ParseObjects(map);
        }

        public Dictionary<string, string> Properties { get; }

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

        private void ProcessObject(SpatialHash<GameObject, GameObjectList> hash, BaseObject obj,
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

        private void ProcessPolygonObject(SpatialHash<GameObject, GameObjectList> hash, PolygonObject obj,
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

        private void ProcessRectangleObject(SpatialHash<GameObject, GameObjectList> hash, RectangleObject obj,
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
            return _collisions.Get(camera.VisibleArea());
        }

        public IEnumerable<GameObject> GetScripts(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _scripts.Get(camera.VisibleArea());
        }

        public IEnumerable<GameObject> GetZones(SCamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));
            return _zones.Get(camera.VisibleArea());
        }
    }
}