using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoVs.Assets.StreamProviders;
using PhotoVs.Assets.TypeLoaders;
using PhotoVs.Logs;

namespace PhotoVs.Assets.AssetLoaders
{
    public class DebugHotReloadAssetLoader : IAssetLoader, IDisposable
    {
        private readonly Dictionary<string, object> _assetCache;
        private readonly FileSystemWatcher _fsWatcher;
        private readonly IStreamProvider _streamProvider;
        private readonly Dictionary<Type, object> _typeLoaders;

        public DebugHotReloadAssetLoader(LoggerCollection logger, IStreamProvider streamProvider)
        {
            Logger = logger;

            Logger.Debug("Initialised DebugHotReloadAssetLoader");
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            _streamProvider = streamProvider;

            _fsWatcher = new FileSystemWatcher
            {
                Path = "\\",
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.*",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            _fsWatcher.Changed += FileWatcher_Changed;
        }

        public T GetAsset<T>(string filepath) where T : class
        {
            filepath = filepath.Replace('/', '\\').ToLowerInvariant();

            if (_assetCache.TryGetValue(filepath, out var asset))
            {
                if (asset != null) return (T) asset;
            }
            else
            {
                LoadAsset<T>(filepath);
                return GetAsset<T>(filepath);
            }

            Logger.Fatal("Could not find asset \"{0}\"", filepath);
            throw new FileNotFoundException();
        }

        public void LoadAsset<T>(string filepath) where T : class
        {
            filepath = filepath.Replace('/', '\\').ToLowerInvariant();

            var loader = _typeLoaders[typeof(T)];
            using var stream = _streamProvider.GetFile(filepath);
            var asset = (loader as ITypeLoader<T>)?.Load(stream);
            if (asset != null)
            {
                Logger.Debug("Loaded asset \"{0}\"", filepath);
                _assetCache[filepath] = asset;
            }
            else
            {
                Logger.Fatal("Could not find asset \"{0}\"", filepath);
                throw new InvalidOperationException();
            }
        }

        public bool UnloadAsset(string filepath)
        {
            filepath = filepath.Replace('/', '\\').ToLowerInvariant();
            _assetCache[filepath] = null;

            var result = _assetCache.Remove(filepath);
            if (result)
                Logger.Debug("Unloaded asset \"{0}\"", filepath);

            return result;
        }

        public bool IsAssetLoaded(string filepath)
        {
            filepath = filepath.Replace('/', '\\').ToLowerInvariant();
            return _assetCache.ContainsKey(filepath);
        }

        public IStreamProvider GetStreamProvider()
        {
            return _streamProvider;
        }

        public IAssetLoader RegisterTypeLoader<T>(ITypeLoader<T> typeLoader)
        {
            if (typeLoader != null)
            {
                Logger.Debug("Registered Type Loader for \"{0}\"", typeof(T).Name);
                _typeLoaders[typeof(T)] = typeLoader;
            }

            return this;
        }

        public LoggerCollection Logger { get; }

        public void Dispose()
        {
            _fsWatcher.Dispose();
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_streamProvider == null)
                return;

            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            var fullName = e.Name;
            var dir = AppDomain.CurrentDomain.BaseDirectory.Substring(3);
            var final = fullName.Replace(dir, "");

            if (final.Count(f => f == '.') > 1)
                final = final.Substring(0, final.LastIndexOf('.'));

            final = final.ToLowerInvariant();

            if (!final.StartsWith(_streamProvider.RootDirectory.ToLowerInvariant()))
                return;

            final = final.Substring(_streamProvider.RootDirectory.Length);

            UnloadAsset(final);
        }
    }
}