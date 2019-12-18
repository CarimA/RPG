using System;
using System.Collections.Generic;
using System.IO;
using PhotoVs.Models.Assets;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class AssetLoader : IAssetLoader
    {
        protected readonly Dictionary<string, object> _assetCache;
        protected readonly IStreamProvider _streamProvider;
        protected readonly Dictionary<Type, object> _typeLoaders;

        public AssetLoader(IStreamProvider streamProvider)
        {
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            _streamProvider = streamProvider;
        }

        public T GetAsset<T>(string filepath) where T : class
        {
            filepath = SanitiseFilename(filepath);
            if (_assetCache.TryGetValue(filepath, out var asset))
            {
                if (asset != null)
                    return (T) asset;
            }
            else
            {
                LoadAsset<T>(filepath);
                return GetAsset<T>(filepath);
            }

            Debug.Log.Fatal("Could not find asset \"{0}\"", filepath);
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

                Debug.Log.Info("Loaded asset \"{0}\"", filepath);
                _assetCache[filepath] = asset;
            }
            else
            {
                Debug.Log.Fatal("Could not find asset \"{0}\"", filepath);
                throw new InvalidOperationException();
            }
        }

        public bool UnloadAsset(string filepath)
        {
            filepath = SanitiseFilename(filepath);

            _assetCache[filepath] = null;

            var result = _assetCache.Remove(filepath);
            if (result)
                Debug.Log.Info("Unloaded asset \"{0}\"", filepath);

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
                Debug.Log.Info("Registered Type Loader for \"{0}\"", typeof(T).Name);
                _typeLoaders[typeof(T)] = typeLoader;
            }

            return this;
        }

        private string SanitiseFilename(string filename)
        {
            return filename.Replace('/', '\\').ToLowerInvariant();
        }
    }
}