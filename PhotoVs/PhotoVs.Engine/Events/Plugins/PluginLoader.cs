﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Events.Plugins.Attributes;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Events.Plugins
{
    public class PluginProvider
    {
        private readonly CoroutineRunner _coroutineRunner;
        private readonly List<string> _namespaces;
        private readonly string _usings;
        private readonly Services _services;
        private readonly EventQueue _events;
        private readonly IAssetLoader _assetLoader;
        private readonly IStreamProvider _streamProvider;
        private readonly List<string> _references;
        private readonly List<Assembly> _assemblies;

        public PluginProvider(Services services)
        {
            _services = services;
            _events = services.Get<EventQueue>();

            _assetLoader = _services.Get<IAssetLoader>();
            _streamProvider = _assetLoader.StreamProvider;
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
                .EnumerateFiles(DataLocation.Content, directory)
                .Where(IsScript);

            if (!scripts.Any())
                return;

            Logger.Write.Info($"Found {scripts.Count()} scripts. ");
            scripts.ForEach(LoadScript);
        }

        public void LoadPluginFromAssembly(Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(IsPlugin);

            foreach (var type in types)
            {
                LoadType("assembly", type);
            }
        }

        public void LoadMods()
        {
            var files = _streamProvider
                .EnumerateFiles(DataLocation.Storage, "mods");

            files.ForEach(LoadPluginsFromZip);
        }

        private void LoadPluginsFromZip(string filename)
        {
            var streamProvider = _assetLoader.StreamProvider;
            using var zip = ZipStorer.Open(streamProvider.Read(DataLocation.Storage, filename), FileAccess.Read, true);
            var files = zip.ReadCentralDir();
            foreach (var file in files)
            {
                using var ms = new MemoryStream();
                zip.ExtractFile(file, ms);
                var script = Encoding.UTF8.GetString(ms.ToArray());
                LoadString("mod: " + filename + " (" + file.FilenameInZip + ")", script);
            }
            zip.Close();
        }

        private void LoadScript(string filename)
        {
            var script = _assetLoader.Get<string>(filename);
            LoadString(filename, script);
        }

        private void LoadString(string source, string script)
        {
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

                Logger.Write.Error($"{failures.Count()} errors found in script \"{source}\"");
                foreach (var error in failures)
                {
                    var line = error.Location.GetLineSpan().StartLinePosition;
                    Logger.Write.Error($"Line {line.Line - _namespaces.Count() - 3} Column {line.Character} {error.Id}: {error.GetMessage()}");
                }
            }
            else
            {
                // reset stream position to 0 to re-read for assembly generation
                ms.Position = 0;

                var assembly = Assembly.Load(ms.GetBuffer()); // AssemblyLoadContext.Default.LoadFromStream(ms);
                var plugins = assembly
                    .GetTypes()
                    .Where(IsPlugin);

                foreach (var plugin in plugins)
                {
                    LoadType(source, plugin);
                }
            }
        }

        private void LoadType(string source, Type type)
        {
            try
            {
                var obj = (IPlugin)Activator.CreateInstance(type, _services);
                var methods = type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(method => method.GetCustomAttributes(typeof(GameEventAttribute), false).Length > 0);

                foreach (var method in methods)
                {
                    var action = (Action<IGameEventArgs>) method.CreateDelegate(typeof(Action<IGameEventArgs>), obj);
                    var runOnce = (method.GetCustomAttributes(typeof(RunOnceAttribute), false).Length > 0);
                    var triggers = method.GetCustomAttributes(typeof(TriggerAttribute), false);

                    // if it's running once, we need to reserve some event ids for each trigger and add a call to unsubscribe for each one
                    if (runOnce)
                    {
                        var ids = Enumerable.Range(0, triggers.Length).Select(_ => _events.Reserve()).ToList();
                        var act = new Action<IGameEventArgs>(obj =>
                        {
                            action(obj);
                            foreach (var id in ids)
                            {
                                _events.Unsubscribe(id);
                            }
                        });

                        for (var i = 0; i < ids.Count; i++)
                        {
                            var t = (TriggerAttribute) triggers[i];
                            _events.Subscribe(ids[i], t.RunOn, act);
                        }

                    }
                    else
                    {
                        foreach (var trigger in triggers)
                        {
                            var t = (TriggerAttribute) trigger;
                            _events.Subscribe(t.RunOn, action);
                        }
                    }
                }

                Logger.Write.Info($"Loaded plugin: {obj.Name}");
            }
            catch (Exception e)
            {
                Logger.Write.Error($"Could not load plugin \"{type.Name}\" from \"{source}\"");
                Logger.Write.Error(e.ToString());
            }
        }

        private bool IsPlugin(Type type)
        {
            return typeof(IPlugin).IsAssignableFrom(type) && type != typeof(IPlugin);
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
                .Where(IsAllowed)
                .ToList();
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