using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PhotoVs.Engine.Assets;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Plugins
{
    public class PluginProvider
    {
        private readonly List<string> _namespaces;
        private readonly string _usings;
        private readonly Services _services;
        private readonly IAssetLoader _assetLoader;
        private readonly IStreamProvider _streamProvider;
        private readonly List<string> _references;
        private readonly List<Assembly> _assemblies;

        public PluginProvider(Services services)
        {
            _services = services;
            _assetLoader = _services.Get<IAssetLoader>();
            _streamProvider = _assetLoader.GetStreamProvider();
            _assemblies = GetAssemblies();
            var types = GetTypesFromAssemblies(_assemblies);
            _namespaces = GetNamespacesFromTypes(types);
            _namespaces.Add("System.Collections");
            _namespaces.Add("System.Runtime");
            _namespaces.Add("System");
            _references = GetReferencesFromAssemblies(_assemblies);
            _usings = GenerateUsings(_namespaces);
        }

        public void LoadPlugins(string directory)
        {
            var scripts = _streamProvider
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
            var script = _assetLoader.GetAsset<string>(filename);
            var code = $@"{_usings}

namespace PhotoVs.Plugins
{{
    {script} 
}}";
            var assemblyName = Path.GetRandomFileName();
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var refs = _references.Select(
                reference => MetadataReference.CreateFromFile(reference));
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: refs,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(
                    diagnostic => diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                Logger.Write.Error($"{failures.Count()} errors found in plugin \"{filename}\"");
                foreach (var error in failures)
                {
                    var line = error.Location.GetLineSpan().StartLinePosition;
                    Logger.Write.Error($"Line {line.Line - _namespaces.Count() - 3} Column {line.Character} {error.Id}: {error.GetMessage()}");
                }
            }
            else
            {
                try
                {
                    // reset stream position to 0 to re-read for assembly generation
                    ms.Position = 0;

                    var assembly = Assembly.Load(ms.GetBuffer());// AssemblyLoadContext.Default.LoadFromStream(ms);
                    var plugins = assembly
                        .GetTypes()
                        .Where(type => typeof(IPlugin).IsAssignableFrom(type));

                    foreach (var plugin in plugins)
                    {
                        var obj = (IPlugin)Activator.CreateInstance(plugin, _services);
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
            return !(reference.StartsWith("Microsoft.GeneratedCode"));
        }

        private List<Assembly> GetAssemblies()
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

        private List<string> GetReferencesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .Select(assembly => assembly.Location)
                .Where(IsAllowed)
                .Distinct()
                .ToList();
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