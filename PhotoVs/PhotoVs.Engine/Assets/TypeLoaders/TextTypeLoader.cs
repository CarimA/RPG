using System.IO;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class TextTypeLoader : ITypeLoader<string>
    {
        public string Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}