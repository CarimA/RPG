using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class AssetLoader : IAssetLoader, IHasBeforeUpdate
    {
        private const int UnloadTime = 8;
        private readonly Dictionary<string, object> _assetCache;
        private readonly Dictionary<string, int> _lastUsed;
        private readonly Dictionary<Type, object> _typeLoaders;
        private float _unloadTimer;

        public AssetLoader(IPlatform platform)
        {
            _platform = platform;
            _lastUsed = new Dictionary<string, int>();
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            StreamProvider = platform.StreamProvider;
        }

        private IPlatform _platform { get; }
        public IStreamProvider StreamProvider { get; }

        public T Get<T>(string filepath) where T : class
        {
            var ext = Path.GetExtension(filepath);
            if (_platform.FileExtensionReplacement.TryGetValue(ext, out var value))
                filepath = filepath.Replace(ext, value);

            filepath = SanitiseFilename(filepath);
            if (_assetCache.TryGetValue(filepath, out var asset))
            {
                _lastUsed[filepath] = Environment.TickCount;
                if (asset != null)
                    return (T) asset;
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
            using var stream = StreamProvider.Read(DataLocation.Content, filepath);
            var asset = Process<T>(stream);
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

        public T Process<T>(Stream stream) where T : class
        {
            var loader = _typeLoaders[typeof(T)];
            var asset = (loader as ITypeLoader<T>)?.Load(stream);
            return asset;
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
                Logger.Write.Info("Registered asset type loader for \"{0}\"", typeof(T).Name);
                _typeLoaders[typeof(T)] = typeLoader;
            }

            return this;
        }

        public int BeforeUpdatePriority { get; set; } = 0;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            _unloadTimer -= gameTime.GetElapsedSeconds();
            if (_unloadTimer <= 0)
            {
                _unloadTimer = UnloadTime;

                var toRemove = new List<string>();

                foreach (var kvp in _lastUsed.Where(kvp => _assetCache.ContainsKey(kvp.Key))
                    .Where(kvp => kvp.Value < Environment.TickCount - UnloadTime * 1000))
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