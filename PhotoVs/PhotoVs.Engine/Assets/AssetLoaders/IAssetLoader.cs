using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public interface IAssetLoader
    {
        IStreamProvider StreamProvider { get; }

        T Get<T>(string filepath) where T : class;
        T GetStorage<T>(string filepath) where T : class;
        void Load<T>(string filepath) where T : class;
        bool Unload(string filepath);

        bool IsLoaded(string filepath);

        IAssetLoader RegisterTypeLoader<T>(ITypeLoader<T> typeLoader);
    }
}