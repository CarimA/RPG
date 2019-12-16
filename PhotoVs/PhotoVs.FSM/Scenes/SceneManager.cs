using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.ECS.Components;
using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;
using PhotoVs.FSM.States;

namespace PhotoVs.FSM.Scenes
{
    public class SceneManager : ISceneManager
    {
        private readonly EntityCollection _globalEntities;
        private readonly SystemCollection _globalSystems;
        private readonly StateMachine<IScene> _scenes;
        private EntityCollection _entitiesCache;
        private int _globalEntitiesHash;

        private int _globalSystemsHash;
        private int _localEntitiesHash;
        private int _localSystemsHash;

        private SystemCollection _systemsCache;

        public SceneManager(StateMachine<IScene> scenes, SystemCollection globalSystems,
            EntityCollection globalEntities)
        {
            _scenes = scenes;
            _globalSystems = globalSystems;
            _globalEntities = globalEntities;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var currentState in _scenes.CurrentStates())
            {
                if (currentState is ISystemScene systemScene)
                {
                    var systems = GetCombinedSystems(systemScene)
                        .Where(IsActive)
                        .OfType<IUpdateableSystem>();
                    var entities = GetCombinedEntities(systemScene);

                    BeforeUpdate(systems, gameTime);
                    Update(systems, entities, gameTime);
                    AfterUpdate(systems, gameTime);
                }

                if (currentState is IUpdateableScene updateableScene) updateableScene.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var currentState in _scenes.CurrentStates())
            {
                if (currentState is ISystemScene systemScene)
                {
                    var systems = GetCombinedSystems(systemScene)
                        .Where(IsActive)
                        .OfType<IDrawableSystem>();
                    var entities = GetCombinedEntities(systemScene);

                    BeforeDraw(systems, gameTime);
                    Draw(systems, entities, gameTime);
                    AfterDraw(systems, gameTime);
                }

                if (currentState is IDrawableScene drawableScene) drawableScene.Draw(gameTime);
            }
        }

        public StateMachine<IScene> GetScenes()
        {
            return _scenes;
        }

        private bool IsActive(ISystem system)
        {
            return system.Active;
        }

        private void BeforeUpdate(IEnumerable<IUpdateableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems) system.BeforeUpdate(gameTime);
        }

        private void Update(IEnumerable<IUpdateableSystem> systems, EntityCollection entities, GameTime gameTime)
        {
            foreach (var system in systems)
                system.Update(gameTime,
                    system.Requires.Contains(typeof(NoComponentRequired))
                        ? entities
                        : entities.All(system.Requires));
        }

        private void AfterUpdate(IEnumerable<IUpdateableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems) system.AfterUpdate(gameTime);
        }

        private void BeforeDraw(IEnumerable<IDrawableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems) system.BeforeDraw(gameTime);
        }

        private void Draw(IEnumerable<IDrawableSystem> systems, EntityCollection entities, GameTime gameTime)
        {
            foreach (var system in systems)
                system.Draw(gameTime,
                    system.Requires.Contains(typeof(NoComponentRequired))
                        ? entities
                        : entities.All(system.Requires));
        }

        private void AfterDraw(IEnumerable<IDrawableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems) system.AfterDraw(gameTime);
        }

        private SystemCollection GetCombinedSystems(ISystemScene currentState)
        {
            if (_globalSystemsHash != _globalSystems.GetUniqueSeed() ||
                _localSystemsHash != currentState.Systems.GetUniqueSeed())
            {
                _globalSystemsHash = _globalSystems.GetUniqueSeed();
                _localSystemsHash = currentState.Systems.GetUniqueSeed();

                // Logger.Debug($"System Cache changed: Global [{_globalSystemsHash}], Local [{_localSystemsHash}]");

                var systems = currentState.Systems.Concat(_globalSystems).ToList();
                systems.Sort(SortByPriority);
                _systemsCache = new SystemCollection(systems);
            }

            return _systemsCache;
        }

        private EntityCollection GetCombinedEntities(ISystemScene currentState)
        {
            if (_globalEntitiesHash != _globalEntities.GetUniqueSeed() ||
                _localEntitiesHash != currentState.Entities.GetUniqueSeed())
            {
                _globalEntitiesHash = _globalEntities.GetUniqueSeed();
                _localEntitiesHash = currentState.Entities.GetUniqueSeed();

                // Logger.Debug($"Entity Cache changed: Global [{_globalEntitiesHash}], Local [{_localEntitiesHash}]");

                var entities = currentState.Entities.Concat(_globalEntities).ToList();
                _entitiesCache = new EntityCollection(entities);
            }

            return _entitiesCache;
        }

        private int SortByPriority(ISystem a, ISystem b)
        {
            return a.Priority.CompareTo(b.Priority);
        }
    }
}