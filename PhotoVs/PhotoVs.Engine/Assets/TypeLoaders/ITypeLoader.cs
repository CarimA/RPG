using System.IO;

namespace PhotoVs.Engine.Assets
{
    public interface ITypeLoader<out T>
    {
        T Load(Stream stream);
    }
}