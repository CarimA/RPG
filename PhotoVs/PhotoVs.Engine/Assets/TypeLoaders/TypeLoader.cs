using System.IO;
using PhotoVs.Engine.Assets.AssetLoaders;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public abstract class TypeLoader<T> : ITypeLoader<T>
    {
        private readonly IAssetLoader _assetLoader;

        public TypeLoader(IAssetLoader assetLoader)
        {
            assetLoader.RegisterTypeLoader<T>(this);
        }

        public abstract T Load(Stream stream);
    }
}
