using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PhotoVs.Events;
using PhotoVs.Extensions;

namespace PhotoVs.Plugins
{
    public class PluginProvider
    {
        private readonly GameEvents _gameEvents;
        private readonly List<Plugin> _plugins;

        public PluginProvider(string directory, GameEvents gameEvents)
        {
            _plugins = new List<Plugin>();
            _gameEvents = gameEvents;

            try
            {
                Directory
                    .GetFiles(directory)
                    .ForEach(file => Assembly
                        .LoadFrom(file)
                        .GetTypes()
                        .Where(type => type.IsAssignableFrom(typeof(Plugin)))
                        .ForEach(LoadAssembly));
            }
            catch (Exception e)
            {

            }

            BindPlugins(gameEvents);
        }

        private void LoadAssembly(Type type)
        {
            _plugins.Add((Plugin) Activator.CreateInstance(type, _gameEvents));
        }

        private void BindPlugins(GameEvents gameEvents)
        {
            _plugins.ForEach(plugin => plugin.Bind(gameEvents));
        }
    }
}