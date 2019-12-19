using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PhotoVs.Engine;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Logic.Services;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Logic.Plugins
{
    public class PluginProvider
    {
        private readonly ServiceLocator _services;
        private readonly List<Plugin> _plugins;

        public PluginProvider(string directory, ServiceLocator services)
        {
            _plugins = new List<Plugin>();
            _services = services;

            LoadPlugins(directory);

            Logger.Write.Info($"Loaded {_plugins.Count} plugin(s)");
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
            var plugin = (Plugin)Activator.CreateInstance(type);
            plugin.Bind(_services.Events);
            plugin.Services = _services;
            _plugins.Add(plugin);
            Logger.Write.Info($"Loaded plugin: {plugin.Name} - v{plugin.Version}");
        }

        public void LoadPlugin(Type plugin)
        {
            LoadAssembly(plugin);
        }
    }
}