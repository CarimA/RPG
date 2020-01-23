using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PhotoVs.Engine;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Logic.Plugins
{
    public class PluginProvider
    {
        private readonly List<Plugin> _plugins;
        private readonly Services _services;
        private readonly Coroutines _coroutines;
        private readonly SceneMachine _sceneMachine;
        private readonly Player _player;

        public PluginProvider(Services services)
        {
            _plugins = new List<Plugin>();
            _services = services;
            _coroutines = _services.Get<Coroutines>();
            _sceneMachine = _services.Get<SceneMachine>();
            _player = _services.Get<Player>();
        }

        public void LoadPlugins(string directory)
        {
            if (Directory.Exists(directory))
            {
                var dlls = Directory.GetFiles(directory);
                foreach (var dll in dlls)
                {
                    if (!dll.EndsWith(".dll")) continue;

                    try
                    {
                        var assembly = Assembly.LoadFrom(dll);
                        Logger.Write.Info($"Loaded dll: {dll}");

                        var types = assembly.GetTypes();
                        var plugins = types.Where(IsPlugin);
                        plugins.ForEach(LoadAssembly);
                    }
                    catch (Exception e)
                    {
                        Logger.Write.Error($"Could not load plugin: {dll}");
                        Logger.Write.Error(e.ToString());
                    }
                }

                Logger.Write.Info($"Loaded {_plugins.Count} plugin(s)");
            }
            else
            {
                Logger.Write.Error($"Could not find {directory}");
            }
        }

        private static bool IsPlugin(Type type)
        {
            return typeof(Plugin).IsAssignableFrom(type) || type.IsSubclassOf(typeof(Plugin));
        }

        private void LoadAssembly(Type type)
        {
            var plugin = (Plugin) Activator.CreateInstance(type);
            plugin.Coroutines = _coroutines;
            plugin.SceneMachine = _sceneMachine;
            plugin.Player = _player;
            plugin.Bind(_services);
            _plugins.Add(plugin);
            Logger.Write.Info($"Loaded plugin: {plugin.Name} - v{plugin.Version}");
        }

        public void LoadPlugin(Type plugin)
        {
            LoadAssembly(plugin);
        }
    }
}