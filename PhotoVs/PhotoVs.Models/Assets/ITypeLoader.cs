using System.IO;

namespace PhotoVs.Models.Assets
{
    public interface ITypeLoader<out T>
    {
        T Load(Stream stream);
    }
}