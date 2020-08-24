using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Scripting
{
    public class ScriptHost<T> : IScriptHost, IStartup
    {
        private readonly IAssetLoader _assetLoader;
        private readonly IInterpreter<T> _interpreter;
        private readonly ScriptData _scriptData;

        public ScriptHost(ScriptData scriptData, IAssetLoader assetLoader, IInterpreter<T> interpreter)
        {
            _scriptData = scriptData;
            _assetLoader = assetLoader;
            _interpreter = interpreter;
        }

        public void Start(IEnumerable<object> bindings)
        {
            LoadScripts();
        }

        public void Reset()
        {
            _interpreter.ClearScripts();
            LoadScripts();
        }

        private void LoadScripts()
        {
            foreach (var dir in _scriptData.Directories)
                LoadScripts(_assetLoader.StreamProvider.EnumerateFiles(dir.Item1, dir.Item2));
        }

        private void LoadScripts(IEnumerable<string> files)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            foreach (var s in files)
                if (s.EndsWith(".lua"))
                    LoadScript(s);
                else if (s.EndsWith(".pvm") || s.EndsWith(".zip")) LoadZip(s);
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
            catch (ScriptRuntimeException e)
            {
                Logger.Write.Error($"Issue with script: {e.DecoratedMessage}");
                throw;
            }
        }

        private bool IsScript(string filename)
        {
            return filename.EndsWith(".lua") || filename.EndsWith(".pvm") || filename.EndsWith(".zip");
        }
    }
}