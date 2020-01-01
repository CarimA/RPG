using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Plugins;
using PhotoVs.Logic.Scenes;
using PhotoVs.Models.Assets;
using PhotoVs.Models.Audio;
using PhotoVs.Models.ECS;
using PhotoVs.Models.Text;

namespace PhotoVs.Logic.Services
{
    public class ServiceLocator
    {
        public Events Events { get; }
        public Coroutines Coroutines { get; private set; }
        public PluginProvider Plugins { get; private set; }

        // monogame
        public GraphicsDevice GraphicsDevice { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        // base engine stuff
        public IAssetLoader AssetLoader { get; private set; }
        public IAudio Audio { get; private set; }
        public ITextDatabase TextDatabase { get; private set; }
        public Renderer Renderer { get; private set; }

        // core logical stuff
        public Player Player { get; private set; }
        public SCamera Camera { get; private set; }
        public SceneMachine SceneMachine { get; private set; }
        public IGameObjectCollection GlobalGameObjects { get; private set; }
        public ISystemCollection GlobalSystems { get; private set; }


        public ServiceLocator(Events events)
        {
            Events = events;
        }

        public void Set<T>(T service)
        {
            foreach (var property in typeof(ServiceLocator).GetProperties())
                if (property.PropertyType.IsAssignableFrom(typeof(T))
                    || property.PropertyType is T)
                {
                    property.SetValue(this, service);
                    Events.RaiseOnServiceSet(service);
                    return;
                }

            throw new ArgumentException($"Service of type {typeof(T).Name} not found");
        }
    }
}