using System.IO;

namespace PhotoVs.Assets.TypeLoaders
{
    public interface ITypeLoader<out T>
    {
        T Load(Stream stream);
    }
}