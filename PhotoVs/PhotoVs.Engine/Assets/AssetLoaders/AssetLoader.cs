using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Utils.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class AssetLoader : IAssetLoader
    {
        public IStreamProvider StreamProvider { get; }

        private readonly Dictionary<string, object> _assetCache;
        private readonly Dictionary<string, int> _lastUsed;
        private readonly Dictionary<Type, object> _typeLoaders;

        public AssetLoader(Coroutines coroutines, IStreamProvider streamProvider)
        {
            _lastUsed = new Dictionary<string, int>();
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            StreamProvider = streamProvider;

            coroutines.Start(UnloadUnusedAssets());
        }

        public T GetStorage<T>(string filepath) where T : class
        {
            var loader = _typeLoaders[typeof(T)];
            using var stream = StreamProvider.Read(DataLocation.Storage, filepath);
            var asset = (loader as ITypeLoader<T>)?.Load(stream);
            return asset;
        }

        public T Get<T>(string filepath) where T : class
        {
            filepath = SanitiseFilename(filepath);
            if (_assetCache.TryGetValue(filepath, out var asset))
            {
                _lastUsed[filepath] = Environment.TickCount;
                if (asset != null)
                    return (T)asset;
            }
            else
            {
                Load<T>(filepath);
                return Get<T>(filepath);
            }

            Logger.Write.Fatal("Could not find asset \"{0}\"", filepath);
            throw new FileNotFoundException();
        }

        public void Load<T>(string filepath) where T : class
        {
            filepath = SanitiseFilename(filepath);
            var loader = _typeLoaders[typeof(T)];
            using var stream = StreamProvider.Read(DataLocation.Content, filepath);
            var asset = (loader as ITypeLoader<T>)?.Load(stream);
            if (asset != null)
            {
                Logger.Write.Info("Loaded asset \"{0}\"", filepath);
                _assetCache[filepath] = asset;
            }
            else
            {
                Logger.Write.Fatal("Could not find asset \"{0}\"", filepath);
                throw new InvalidOperationException();
            }
        }

        public bool Unload(string filepath)
        {
            filepath = SanitiseFilename(filepath);

            _assetCache[filepath] = null;

            var result = _assetCache.Remove(filepath);
            if (result)
                Logger.Write.Info("Unloaded asset \"{0}\"", filepath);

            return result;
        }

        public bool IsLoaded(string filepath)
        {
            filepath = SanitiseFilename(filepath);
            return _assetCache.ContainsKey(filepath);
        }

        public IAssetLoader RegisterTypeLoader<T>(ITypeLoader<T> typeLoader)
        {
            if (typeLoader != null)
            {
                Logger.Write.Info("Registered Type Loader for \"{0}\"", typeof(T).Name);
                _typeLoaders[typeof(T)] = typeLoader;
            }

            return this;
        }

        private IEnumerator UnloadUnusedAssets()
        {
            var time = 8f;
            var pause = new Pause(time);
            var toRemove = new List<string>();

            while (true)
            {
                yield return pause;
                pause.Time = time;

                foreach (var kvp in _lastUsed.Where(kvp => _assetCache.ContainsKey(kvp.Key))
                    .Where(kvp => kvp.Value < Environment.TickCount - 8000))
                {
                    toRemove.Add(kvp.Key);
                    Unload(kvp.Key);
                }

                foreach (var key in toRemove)
                    _lastUsed.Remove(key);

                toRemove.Clear();
            }
        }

        private string SanitiseFilename(string filename)
        {
            return filename.ToLowerInvariant();
        }
    }
}