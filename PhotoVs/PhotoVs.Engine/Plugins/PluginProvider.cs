using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace PhotoVs.Engine.Plugins
{
    public class PluginProvider
    {
        private readonly Services _services;

        public PluginProvider(Services services)
        {
            _services = services;
        }

        public void LoadPluginFromAssembly(Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(IsPlugin);

            types.ForEach(LoadType);
        }

        private void LoadType(Type type)
        {
            try
            {
                var plugin = (IPlugin)Activator.CreateInstance(type, _services);
                Logger.Write.Info($"Loaded plugin: {plugin.Name} - {plugin.Version} ");
            }
            catch (Exception e)
            {
                Logger.Write.Error($"Could not load plugin \"{type.Name}\"");
                Logger.Write.Error(e.ToString());
            }
        }

        private bool IsPlugin(Type type)
        {
            return typeof(IPlugin).IsAssignableFrom(type);
        }
    }
}
