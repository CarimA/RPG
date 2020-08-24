using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Core;

namespace PhotoVs.Engine.ECS
{
    // todo: bind systems and system collection to kernel

    public class Stage : IStartup, IHasUpdate, IHasDraw
    {
        private List<Scene> _scenes;
        private Scene _currentScene;
        public Scene CurrentScene => _currentScene;

        private readonly GameObjectList _gameObjects;
        public GameObjectList GameObjects => _gameObjects;

        private readonly SystemList<System> _globalUpdateSystems;

        public Stage()
        {
            _scenes = new List<Scene>();
            _gameObjects = new GameObjectList();
            _globalUpdateSystems = new SystemList<System>();
        }

        public void Start(IEnumerable<object> bindings)
        {
            _scenes = bindings.Where(scene => scene is Scene)
                .Cast<Scene>()
                .ToList();
        }

        public void RegisterGlobalSystem(Action<GameTime, GameObjectList> method, int priority = 0)
        {
            var system = new System(method, priority);
            if (system.RunOn == RunOn.Update)
                _globalUpdateSystems.Add(system);
            else if (system.RunOn == RunOn.Draw)
                throw new ArgumentException("Global Systems only support Update Systems.", nameof(method));
            else
                throw new ArgumentException("Method has an invalid RunOn value.", nameof(method));
        }

        public void ChangeScene<T>() where T : Scene
        {
            _currentScene?.Exit(GameObjects);
            _currentScene = _scenes.Find(s => s is T);
            _currentScene?.Enter(GameObjects);
        }

        public int UpdatePriority { get; set; } = 0;
        public bool UpdateEnabled { get; set; } = true;

        public void Update(GameTime gameTime)
        {
            foreach (var system in _globalUpdateSystems)
            {
                system.Method(gameTime, system.RequiredComponents.Length == 0
                    ? GameObjects
                    : GameObjects.All(system.RequiredComponents));
            }

            _currentScene?.Update(gameTime, GameObjects);
        }

        public int DrawPriority { get; set; } = 0;
        public bool DrawEnabled { get; set; } = true;

        public void Draw(GameTime gameTime)
        {
            _currentScene?.Draw(gameTime, GameObjects);
        }
    }
}