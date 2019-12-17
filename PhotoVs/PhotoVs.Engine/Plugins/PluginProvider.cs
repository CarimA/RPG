using PhotoVs.Utils.Extensions;
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

            try
            {
                Directory
                    .GetFiles(directory)
                    .ForEach(file => Assembly
                        .LoadFrom(file)
                        .GetTypes()
                        .Where(type => type.IsAssignableFrom(typeof(IPlugin)))
                        .ForEach(LoadAssembly));
            }
            catch (Exception e)
            {
            }

            BindPlugins(gameEvents);
        }

        private void LoadAssembly(Type type)
        {
            _plugins.Add((IPlugin)Activator.CreateInstance(type, _gameEvents));
        }

        private void BindPlugins(Events gameEvents)
        {
            _plugins.ForEach(plugin => plugin.Bind(gameEvents));
        }
    }
}