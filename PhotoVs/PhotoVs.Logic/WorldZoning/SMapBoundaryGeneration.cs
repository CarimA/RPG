using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.Components;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Logic.Camera;
using PhotoVs.Models.ECS;
using System;
using System.Collections.Generic;

namespace PhotoVs.Logic.WorldZoning
{
    public class SMapBoundaryGeneration : IUpdateableSystem
    {
        private readonly SCamera _camera;
        private readonly World _world;
        private IGameObjectCollection _collisions;

        private List<Map> _maps;
        private IGameObjectCollection _scripts;
        private IGameObjectCollection _zones;

        public SMapBoundaryGeneration(World world, SCamera camera)
        {
            _world = world;
            _camera = camera;
        }

        public int Priority { get; set; } = -9999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { };

        public void BeforeUpdate(GameTime gameTime)
        {
            (_maps, _collisions, _scripts, _zones) = _world.GetMap("maps\\test").GetDataInCamera(_camera);
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        public List<Map> GetMaps()
        {
            return _maps;
        }

        public IGameObjectCollection GetCollisions()
        {
            return _collisions;
        }

        public IGameObjectCollection GetScripts()
        {
            return _scripts;
        }

        public IGameObjectCollection GetZones()
        {
            return _zones;
        }
    }
}