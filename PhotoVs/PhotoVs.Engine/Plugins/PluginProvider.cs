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
        private readonly List<IPlugin> _plugins;

        public PluginProvider(string directory, Events gameEvents)
        {
            _plugins = new List<IPlugin>();
            _gameEvents = gameEvents;

            if (Directory.Exists(directory))
            {
                var dlls = Directory.GetFiles(directory);
                foreach (var dll in dlls)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(dll);
                        var types = assembly.GetTypes();
                        var plugins = types.Where(type
                            => type.GetInterfaces().Contains(typeof(IPlugin)));
                        plugins.ForEach(LoadAssembly);
                        Debug.Log.Info($"Loaded plugin: {dll}");
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
            Debug.Log.Info($"Loaded {_plugins.Count} plugin(s)");
        }

        private void LoadAssembly(Type type)
        {
            var plugin = (IPlugin)Activator.CreateInstance(type, _gameEvents);
            plugin.Bind(_gameEvents);
            _plugins.Add(plugin);
        }
    }
}