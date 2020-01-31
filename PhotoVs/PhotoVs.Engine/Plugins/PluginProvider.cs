using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Engine.Plugins
{
    public class PluginProvider
    {
        private readonly Services _services;

        public PluginProvider(Services services)
        {
            _services = services;
        }

        public void LoadPluginsFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public void LoadPluginsFromCode(string code)
        {
            var assemblyName = Path.GetRandomFileName();
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(
                    diagnostic => diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                //Logger.Write.Error($"{failures.Count()} errors found in plugin \"{filename}\"");
                foreach (var error in failures)
                {
                    var line = error.Location.GetLineSpan().StartLinePosition;
                    Logger.Write.Error($"Line {line.Line} Column {line.Character} {error.Id}: {error.GetMessage()}");
                }
            }
            else
            {
                ms.Position = 0;
                var assembly = Assembly.Load(ms.GetBuffer());
                LoadPluginFromAssembly(assembly);
            }
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
