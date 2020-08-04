using System.IO;
using PhotoVs.Engine.Assets.AssetLoaders;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class DynamicSpriteFontTypeLoader : TypeLoader<DynamicSpriteFont>
    {
        public int FontSize { get; set; } = 20;

        public DynamicSpriteFontTypeLoader(IAssetLoader assetLoader) : base(assetLoader)
        {

        }

        public override DynamicSpriteFont Load(Stream stream)
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            var bytes = memory.ToArray();
            var font = DynamicSpriteFont.FromTtf(bytes, FontSize);
            return font;
        }
    }
}