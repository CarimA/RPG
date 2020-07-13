using System.IO;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public interface IAssetLoader
    {
        IStreamProvider StreamProvider { get; }

        T Get<T>(string filepath) where T : class;
        void Load<T>(string filepath) where T : class;
        bool Unload(string filepath);

        T Process<T>(Stream stream) where T : class;

        bool IsLoaded(string filepath);

        IAssetLoader RegisterTypeLoader<T>(ITypeLoader<T> typeLoader);
    }
}