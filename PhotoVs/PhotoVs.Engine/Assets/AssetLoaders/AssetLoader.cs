using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class AssetLoader : IAssetLoader
    {
        protected readonly Dictionary<string, object> _assetCache;
        protected readonly Dictionary<string, int> _lastUsed;
        protected readonly IStreamProvider _streamProvider;
        protected readonly Dictionary<Type, object> _typeLoaders;

        public AssetLoader(Coroutines coroutines, IStreamProvider streamProvider)
        {
            _lastUsed = new Dictionary<string, int>();
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            _streamProvider = streamProvider;

            coroutines.Start(UnloadUnusedAssets());
        }

        public T GetAsset<T>(string filepath) where T : class
        {
            filepath = SanitiseFilename(filepath);
            if (_assetCache.TryGetValue(filepath, out var asset))
            {
                _lastUsed[filepath] = Environment.TickCount;
                if (asset != null)
                    return (T) asset;
            }
            else
            {
                LoadAsset<T>(filepath);
                return GetAsset<T>(filepath);
            }

            Logger.Write.Fatal("Could not find asset \"{0}\"", filepath);
            throw new FileNotFoundException();
        }

        public void LoadAsset<T>(string filepath) where T : class
        {
            filepath = SanitiseFilename(filepath);
            var loader = _typeLoaders[typeof(T)];
            using var stream = _streamProvider.GetFile(filepath);
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

        public bool UnloadAsset(string filepath)
        {
            filepath = SanitiseFilename(filepath);

            _assetCache[filepath] = null;

            var result = _assetCache.Remove(filepath);
            if (result)
                Logger.Write.Info("Unloaded asset \"{0}\"", filepath);

            return result;
        }

        public bool IsAssetLoaded(string filepath)
        {
            filepath = SanitiseFilename(filepath);
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

                foreach (var kvp in _lastUsed)
                {
                    if (!_assetCache.ContainsKey(kvp.Key))
                        continue;

                    if (kvp.Value < Environment.TickCount - 8000)
                    {
                        toRemove.Add(kvp.Key);
                        UnloadAsset(kvp.Key);
                    }
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