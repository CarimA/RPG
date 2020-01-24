using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Models.Assets;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Logic.Plugins
{
    public class PluginProvider
    {
        private readonly CSharpCodeProvider _provider;
        private readonly CompilerParameters _parameters;
        private readonly List<string> _namespaces;
        private readonly string _usings;
        private readonly Services _services;

        public PluginProvider(Services services)
        {
            _services = services;
            var assemblies = GetAssemblies();
            var types = GetTypesFromAssemblies(assemblies);
            _namespaces = GetNamespacesFromTypes(types);
            _namespaces.Add("System.Collections");
            var references = GetReferencesFromAssemblies(assemblies);
            _usings = GenerateUsings(_namespaces);
            (_provider, _parameters) = SetupCompiler(references);
        }

        public void LoadPlugins(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Logger.Write.Error($"Could not find \"{directory}\" to load plugins from.");
                return;
            }

            var scripts = Directory
                .GetFiles(directory)
                .Where(IsScript);

            if (scripts.Count() == 0)
                return;

            Logger.Write.Info($"Found {scripts.Count()} scripts. ");
            scripts.ForEach(LoadScript);
        }

        public void LoadMods()
        {
            var modDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVs/Mods");
            LoadPlugins(modDirectory);
        }

        private void LoadScript(string filename)
        {
            var script = File.ReadAllText(filename);
            var code = $@"{_usings}

namespace PhotoVs.Plugins
{{
    {script} 
}}";
            var results = _provider.CompileAssemblyFromSource(_parameters, code);

            if (results.Errors.HasErrors)
            {
                Logger.Write.Error($"{results.Errors.Count} errors found in plugin \"{filename}\"");
                foreach (CompilerError error in results.Errors)
                {
                    Logger.Write.Error($"L{error.Line - (_namespaces.Count() + 4)} ({error.ErrorNumber}): {error.ErrorText}");
                }
            }
            else
            {
                try
                {
                    var assembly = results.CompiledAssembly;
                    var plugins = assembly
                        .GetTypes()
                        .Where(type => typeof(Plugin).IsAssignableFrom(type));

                    foreach (var plugin in plugins)
                    {
                        var obj = (Plugin)Activator.CreateInstance(plugin, _services);
                        obj.Services = _services;
                        Logger.Write.Info($"Loaded plugin: {obj.Name} - v{obj.Version}");
                    }
                }
                catch (Exception e)
                {
                    Logger.Write.Error($"Could not load plugin \"{filename}\"");
                    Logger.Write.Error(e.ToString());
                }
            }
        }

        private bool IsScript(string filename)
        {
            return filename.EndsWith(".cs");
        }

        private bool IsAllowed(Assembly assembly)
        {
            return IsAllowed(assembly.FullName);
        }

        private bool IsAllowed(string reference)
        {
            return !(reference.StartsWith("mscorlib")
                || reference.StartsWith("Microsoft.GeneratedCode")
                || reference.StartsWith("Microsoft.VisualStudio")
                || reference.StartsWith("Accessibility")
                || reference.StartsWith("System.Runtime"));
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Distinct()
                .Where(IsAllowed);
        }

        private IEnumerable<Type> GetTypesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Distinct();
        }

        private List<string> GetNamespacesFromTypes(IEnumerable<Type> types)
        {
            return types
                .Select(
                type =>
                {
                    if (type.Namespace != null)
                        return type.Namespace;

                    return "";
                })
                .Where(s => !string.IsNullOrEmpty(s))
                .Where(IsAllowed)
                .Distinct()
                .ToList();
        }

        private IEnumerable<string> GetReferencesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(assembly => assembly.GetReferencedAssemblies()
                    .Select(a => a.Name + ".dll"))
                .Where(IsAllowed)
                .Distinct();
        }

        private (CSharpCodeProvider, CompilerParameters) SetupCompiler(IEnumerable<string> references)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.AddRange(references.ToArray());

            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            return (provider, parameters);
        }

        private string GenerateUsings(IEnumerable<string> namespaces)
        {
            var output = "";
            foreach (var ns in namespaces)
            {
                output += $"using {ns};\r\n";
            }
            return output;
        }
    }
}