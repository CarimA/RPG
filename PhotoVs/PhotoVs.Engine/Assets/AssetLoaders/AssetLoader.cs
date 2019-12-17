using System;
using System.Collections.Generic;
using System.IO;
using PhotoVs.Models.Assets;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class AssetLoader : IAssetLoader
    {
        private readonly Dictionary<string, object> _assetCache;
        private readonly IStreamProvider _streamProvider;
        private readonly Dictionary<Type, object> _typeLoaders;

        public AssetLoader(IStreamProvider streamProvider)
        {
            _assetCache = new Dictionary<string, object>();
            _typeLoaders = new Dictionary<Type, object>();
            _streamProvider = streamProvider;
        }

        public T GetAsset<T>(string filepath) where T : class
        {
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

            throw new FileNotFoundException();
        }

        public void LoadAsset<T>(string filepath) where T : class
        {
            var loader = _typeLoaders[typeof(T)];
            using var stream = _streamProvider.GetFile(filepath);
            var asset = (loader as ITypeLoader<T>)?.Load(stream);
            if (asset != null)
                _assetCache[filepath] = asset;
            else
                throw new InvalidOperationException();
        }

        public bool UnloadAsset(string filepath)
        {
            return _assetCache.Remove(filepath);
        }

        public bool IsAssetLoaded(string filepath)
        {
            return _assetCache.ContainsKey(filepath);
        }

        public IStreamProvider GetStreamProvider()
        {
            return _streamProvider;
        }

        public IAssetLoader RegisterTypeLoader<T>(ITypeLoader<T> typeLoader)
        {
            if (typeLoader != null)
                _typeLoaders[typeof(T)] = typeLoader;

            return this;
        }
    }
}