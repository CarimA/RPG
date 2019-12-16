using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.CommonGameLogic.Camera;
using PhotoVs.Core.ECS.Entities;
using PhotoVs.ECS.Components;
using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;
using PhotoVs.TiledMaps;

namespace PhotoVs.WorldZoning
{
    public class SMapBoundaryGeneration : IUpdateableSystem
    {
        private readonly SCamera _camera;
        private readonly World _world;
        private List<IEntity> _collisions;

        private List<Map> _maps;
        private List<IEntity> _scripts;
        private List<IEntity> _zones;

        public SMapBoundaryGeneration(World world, SCamera camera)
        {
            _world = world;
            _camera = camera;
        }

        public int Priority { get; set; } = -9999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(NoComponentRequired) };

        public void BeforeUpdate(GameTime gameTime)
        {
            (_maps, _collisions, _scripts, _zones) = _world.GetMap("maps\\test").GetDataInCamera(_camera);
        }

        public void Update(GameTime gameTime, EntityCollection entities)
        {
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        public List<Map> GetMaps()
        {
            return _maps;
        }

        public List<IEntity> GetCollisions()
        {
            return _collisions;
        }

        public List<IEntity> GetScripts()
        {
            return _scripts;
        }

        public List<IEntity> GetZones()
        {
            return _zones;
        }
    }
}