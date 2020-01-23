using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;
using YamlDotNet.Core;

namespace PhotoVs.Logic
{
    public class Services
    {
        private readonly Dictionary<Type, object> _cache;

        public Services()
        {
            _cache = new Dictionary<Type, object>();
        }

        public T Get<T>()
        {
            if (_cache.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException();
        }

        public void Set<T>(T service)
        {
            _cache.Add(typeof(T), service);
            Logger.Write.Trace($"Registered Type \"{typeof(T).Name}\" as service.");
        }
    }
}