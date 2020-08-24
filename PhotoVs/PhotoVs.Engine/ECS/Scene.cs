using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.ECS
{
    public abstract class Scene
    {
        private readonly SystemList<StartupSystem> _enterSystems;
        private readonly SystemList<StartupSystem> _exitSystems;
        private readonly SystemList<System> _updateSystems;
        private readonly SystemList<System> _drawSystems;
        private readonly Stack<Scene> _subScenes;

        public Scene()
        {
            _enterSystems = new SystemList<StartupSystem>();
            _exitSystems = new SystemList<StartupSystem>();
            _updateSystems = new SystemList<System>();
            _drawSystems = new SystemList<System>();
            _subScenes = new Stack<Scene>();
        }

        public void PushSubScene(Scene subScene, GameObjectList gameObjects)
        {
            _subScenes.Push(subScene);
            subScene.Enter(gameObjects);
        }

        public Scene PopSubScene(GameObjectList gameObjects)
        {
            _subScenes.Peek().Exit(gameObjects);
            return _subScenes.Pop();
        }

        public Scene PeekSubScene()
        {
            return _subScenes.Peek();
        }

        public void ClearSubScenes(GameObjectList gameObjects)
        {
            while (_subScenes.Count > 0)
                PopSubScene(gameObjects);

            _subScenes.Clear();
        }

        public void RegisterSystem(Action<GameTime, GameObjectList> method, int priority = 0)
        {
            var system = new System(method, priority);
            if (system.RunOn == RunOn.Update)
                _updateSystems.Add(system);
            else if (system.RunOn == RunOn.Draw)
                _drawSystems.Add(system);
            else
                throw new ArgumentException("Method has an invalid RunOn value.", nameof(method));
        }

        public void RegisterStartupSystem(Action<GameObjectList> method, int priority = 0)
        {
            var system = new StartupSystem(method, priority);
            if (system.RunOn == RunOn.Enter)
                _enterSystems.Add(system);
            else if (system.RunOn == RunOn.Exit)
                _exitSystems.Add(system);
            else
                throw new ArgumentException("Method has an invalid RunOn value.", nameof(method));
        }

        public void Enter(GameObjectList gameObjects)
        {
            foreach (var system in _enterSystems)
            {
                system.Method(system.RequiredComponents.Length == 0
                    ? gameObjects
                    : gameObjects.All(system.RequiredComponents));
            }
        }

        public void Exit(GameObjectList gameObjects)
        {
            foreach (var system in _exitSystems)
            {
                system.Method(system.RequiredComponents.Length == 0
                    ? gameObjects
                    : gameObjects.All(system.RequiredComponents));
            }

            ClearSubScenes(gameObjects);
        }

        public void Update(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var system in _updateSystems)
            {
                system.Method(gameTime, 
                    system.RequiredComponents.Length == 0
                        ? gameObjects
                        : gameObjects.All(system.RequiredComponents));
            }

            foreach (var subScene in _subScenes)
            {
                subScene.Update(gameTime, gameObjects);
            }
        }

        public void Draw(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var system in _drawSystems)
            {
                system.Method(gameTime,
                    system.RequiredComponents.Length == 0
                        ? gameObjects
                        : gameObjects.All(system.RequiredComponents));
            }

            foreach (var subScene in _subScenes)
            {
                subScene.Draw(gameTime, gameObjects);
            }
        }
    }
}