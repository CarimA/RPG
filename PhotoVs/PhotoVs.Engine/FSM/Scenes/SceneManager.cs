using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.FSM.States;
using PhotoVs.Utils.Logging;

/* Unmerged change from project 'PhotoVs.Engine (netcoreapp3.1)'
Before:
using System.Linq;
using PhotoVs.Engine.ECS;
using PhotoVs.Utils.Logging;
After:
using System.Collections.Generic;
using System.Linq;
*/

/* Unmerged change from project 'PhotoVs.Engine (monoandroid8)'
Before:
using System.Linq;
using PhotoVs.Engine.ECS;
using PhotoVs.Utils.Logging;
After:
using System.Collections.Generic;
using System.Linq;
*/
using System.Linq;

namespace PhotoVs.Engine.FSM.Scenes
{
    public class SceneManager : ISceneManager
    {
        private readonly GameObjectList _globalEntities;
        private readonly ISystemCollection<ISystem> _globalSystems;
        private readonly StateMachine<IScene> _scenes;

        public SceneManager(StateMachine<IScene> scenes, ISystemCollection<ISystem> globalSystems,
            GameObjectList globalEntities)
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

                if (currentState is IUpdateableScene updateableScene)
                    updateableScene.Update(gameTime);
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

                if (currentState is IDrawableScene drawableScene)
                    drawableScene.Draw(gameTime);
            }
        }

        public void DrawUI(GameTime gameTime, Matrix uiOrigin)
        {
            foreach (var currentState in _scenes.CurrentStates())
            {
                if (currentState is ISystemScene systemScene)
                {
                    var systems = GetCombinedSystems(systemScene)
                        .Where(IsActive)
                        .OfType<IDrawableSystem>();
                    var entities = GetCombinedEntities(systemScene);

                    DrawUI(systems, entities, gameTime, uiOrigin);
                }

                if (currentState is IDrawableScene drawableScene)
                    drawableScene.DrawUI(gameTime, uiOrigin);
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
            foreach (var system in systems)
                system.BeforeUpdate(gameTime);
        }

        private void Update(IEnumerable<IUpdateableSystem> systems, GameObjectList entities, GameTime gameTime)
        {
            foreach (var system in systems)
                system.Update(gameTime,
                    system.Requires.Length == 0
                        ? entities
                        : entities.All(system.Requires));
        }

        private void AfterUpdate(IEnumerable<IUpdateableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.AfterUpdate(gameTime);
        }

        private void BeforeDraw(IEnumerable<IDrawableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.BeforeDraw(gameTime);
        }

        private void Draw(IEnumerable<IDrawableSystem> systems, GameObjectList entities, GameTime gameTime)
        {
            foreach (var system in systems)
                system.Draw(gameTime,
                    system.Requires.Length == 0
                        ? entities
                        : entities.All(system.Requires));
        }

        private void AfterDraw(IEnumerable<IDrawableSystem> systems, GameTime gameTime)
        {
            foreach (var system in systems)
                system.AfterDraw(gameTime);
        }

        private void DrawUI(IEnumerable<IDrawableSystem> systems, GameObjectList entities, GameTime gameTime, Matrix uiOrigin)
        {
            foreach (var system in systems)
                system.DrawUI(gameTime,
                    system.Requires.Length == 0
                        ? entities
                        : entities.All(system.Requires),
                    uiOrigin);
        }

        private ISystemCollection<ISystem> GetCombinedSystems(ISystemScene currentState)
        {
            var systems = currentState.Systems.Concat(_globalSystems).ToList();
            systems.Sort(SortByPriority);
            return new SystemCollection<ISystem>(systems);
        }

        private GameObjectList GetCombinedEntities(ISystemScene currentState)
        {
            return new GameObjectList(currentState.Entities.Concat(_globalEntities).ToList());
        }

        private int SortByPriority(ISystem a, ISystem b)
        {
            return a.Priority.CompareTo(b.Priority);
        }
    }
}