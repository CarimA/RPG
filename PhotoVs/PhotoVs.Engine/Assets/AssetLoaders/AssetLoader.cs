using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Utils.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.Coroutines.Instructions;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class AssetLoader : IAssetLoader
    {
        public IStreamProvider StreamProvider { get; }

        private IPlatform _platform { get; }
        private readonly Dictionary<string, object> _assetCache;
        private readonly Dictionary<string, int> _lastUsed;
        private readonly Dictionary<Type, object> _typeLoaders;

        public AssetLoader(Services services, IStreamProvider streamProvider)
        {
            _platform = services.Get<IPlatform>();
            _lastUsed = new Dictionary<string, int>();
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            StreamProvider = streamProvider;

            services.Get<CoroutineRunner>().Start(new Coroutine(UnloadUnusedAssets()));
        }

        public T Get<T>(string filepath) where T : class
        {
            if (filepath.EndsWith(".fx"))
                filepath = filepath.Replace(".fx", _platform.ShaderFileExtension);

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

        private string SanitiseFilename(string filename)
        {
            return filename.ToLowerInvariant();
        }

        private IEnumerator UnloadUnusedAssets()
        {
            const float time = 20f;
            var pause = new Wait(time);
            var toRemove = new List<string>();

            while (true)
            {
                yield return pause;
                pause.SetTime(time);

                foreach (var kvp in _lastUsed.Where(kvp => _assetCache.ContainsKey(kvp.Key))
                    .Where(kvp => kvp.Value < Environment.TickCount - (time * 1000)))
                {
                    toRemove.Add(kvp.Key);
                    Unload(kvp.Key);
                }

                foreach (var key in toRemove)
                    _lastUsed.Remove(key);

                toRemove.Clear();
            }
        }
    }
}