using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Coroutine = PhotoVs.Engine.Events.Coroutines.Coroutine;

namespace PhotoVs.Engine.Scripting
{
    public class ScriptHost
    {
        private readonly CoroutineRunner _coroutines;
        private readonly IAssetLoader _assetLoader;

        private MoonSharpInterpreter _interpreter;
        private readonly List<Module> _modules;
        private readonly IEnumerable<(DataLocation, string)> _scriptDirectories;

        public ScriptHost(Services services, IEnumerable<(DataLocation, string)> scriptDirectories, List<Module> modules)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            _scriptDirectories = scriptDirectories ?? throw new ArgumentNullException(nameof(scriptDirectories));
            _assetLoader = services.Get<IAssetLoader>();
            _interpreter = new MoonSharpInterpreter();
            _coroutines = services.Get<CoroutineRunner>();
            _modules = modules;
            foreach (var module in _modules)
                module.AttachScriptHost(this);

            LoadScripts();
        }

        public void Reset()
        {
            _interpreter = new MoonSharpInterpreter();
            LoadScripts();
        }

        private void LoadScripts()
        {
            DefineApi();

            foreach (var dir in _scriptDirectories)
            {
                LoadScripts(_assetLoader.StreamProvider.EnumerateFiles(dir.Item1, dir.Item2));
            }
        }

        private void LoadScripts(IEnumerable<string> files)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            foreach (var s in files)
            {
                if (s.EndsWith(".lua"))
                {
                    LoadScript(s);
                }
                else if (s.EndsWith(".pvm") || s.EndsWith(".zip"))
                {
                    LoadZip(s);
                }
            }
        }

        private void LoadScript(string filename)
        {
            var script = _assetLoader.Get<string>(filename);
            LoadString(filename, script);
        }

        private void LoadZip(string filename)
        {
            var streamProvider = _assetLoader.StreamProvider;
            using var zip = ZipStorer.Open(streamProvider.Read(DataLocation.Storage, filename), FileAccess.Read, true);
            var files = zip.ReadCentralDir();
            foreach (var file in files)
            {
                using var ms = new MemoryStream();
                zip.ExtractFile(file, ms);
                var script = Encoding.UTF8.GetString(ms.ToArray());
                LoadString($"mod: {filename} ({file.FilenameInZip})", script);
            }
            zip.Close();
        }

        private void LoadString(string source, string script)
        {
            try
            {
                _interpreter.RunScript(script);
            }
            catch (Exception e)
            {
                Logger.Write.Error($"Issue with script: {source}");
                throw;
            }
        }

        private bool IsScript(string filename)
        {
            return filename.EndsWith(".lua") || filename.EndsWith(".pvm") || filename.EndsWith(".zip");
        }

        private void DefineApi()
        {
            foreach (var module in _modules)
                module.DefineApi(_interpreter);

            _interpreter.AddFunction("print", (Action<string, object[]>)Logger.Write.Trace);

        }

        public void RunCoroutine(string source, Closure closure)
        {
            var coroutine = _interpreter.Script.CreateCoroutine(closure);
            var iterator = coroutine.Coroutine.AsUnityCoroutine();
            _coroutines.Start(new Coroutine(source, iterator));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var module in _modules)
                module.Update(gameTime);
        }
    }
}
