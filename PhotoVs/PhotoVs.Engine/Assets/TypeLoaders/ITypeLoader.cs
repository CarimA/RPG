using System.IO;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public interface ITypeLoader<out T>
    {
        T Load(Stream stream);
    }
}