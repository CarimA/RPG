using PhotoVs.Engine.Scheduler;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhotoVs.Engine.Plugins
{
    public class PluginProvider
    {
        private readonly Events _gameEvents;
        private readonly Coroutines _coroutines;
        private readonly List<Plugin> _plugins;

        public PluginProvider(string directory, Events gameEvents, Coroutines coroutines)
        {
            _plugins = new List<Plugin>();
            _gameEvents = gameEvents;
            _coroutines = coroutines;

            LoadPlugins(directory);

            Debug.Log.Info($"Loaded {_plugins.Count} plugin(s)");
        }

        private void LoadPlugins(string directory)
        {
            if (Directory.Exists(directory))
            {
                var dlls = Directory.GetFiles(directory);
                foreach (var dll in dlls)
                {
                    if (!dll.EndsWith(".dll"))
                    {
                        continue;
                    }

                    try
                    {
                        var assembly = Assembly.LoadFrom(dll);
                        Debug.Log.Info($"Loaded dll: {dll}");

                        var types = assembly.GetTypes();
                        var plugins = types.Where(IsPlugin);
                        plugins.ForEach(LoadAssembly);
                    }
                    catch (Exception e)
                    {
                        Debug.Log.Error($"Could not load plugin: {dll}");
                        Debug.Log.Error(e.ToString());
                    }
                }
            }
            else
            {
                Debug.Log.Error($"Could not find {directory}");
            }
        }

        private static bool IsPlugin(Type type)
        {
            return typeof(Plugin).IsAssignableFrom(type) || type.IsSubclassOf(typeof(Plugin));
        }

        private void LoadAssembly(Type type)
        {
            var plugin = (Plugin)Activator.CreateInstance(type);
            plugin.Bind(_gameEvents);
            plugin.Coroutines = _coroutines;
            _plugins.Add(plugin);
            Debug.Log.Info($"Loaded plugin: {plugin.Name} - v{plugin.Version}");
        }

        public void LoadPlugin<T>(T plugin) where T : Plugin
        {
            LoadAssembly(plugin.GetType());
        }
    }
}