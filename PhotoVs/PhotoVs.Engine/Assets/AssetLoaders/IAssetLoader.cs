namespace PhotoVs.Models.Assets
{
    public interface IAssetLoader
    {
        T GetAsset<T>(string filepath) where T : class;
        void LoadAsset<T>(string filepath) where T : class;
        bool UnloadAsset(string filepath);

        bool IsAssetLoaded(string filepath);

        IStreamProvider GetStreamProvider();
        IAssetLoader RegisterTypeLoader<T>(ITypeLoader<T> typeLoader);
    }
}