using System.IO;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class DynamicSpriteFontTypeLoader : ITypeLoader<DynamicSpriteFont>
    {
        private readonly int _defaultFontSize;

        public DynamicSpriteFontTypeLoader(int defaultFontSize)
        {
            _defaultFontSize = defaultFontSize;
        }

        public DynamicSpriteFont Load(Stream stream)
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            var bytes = memory.ToArray();
            var font = DynamicSpriteFont.FromTtf(bytes, _defaultFontSize);
            return font;
        }
    }
}