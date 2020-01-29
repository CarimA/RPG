﻿using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets;
using SpriteFontPlus;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class SpriteFontTypeLoader : ITypeLoader<SpriteFont>
    {
        private readonly IAssetLoader _assetLoader;
        private readonly GraphicsDevice _graphicsDevice;

        public SpriteFontTypeLoader(GraphicsDevice graphicsDevice, IAssetLoader loader)
        {
            _graphicsDevice = graphicsDevice;
            _assetLoader = loader;
        }

        public SpriteFont Load(Stream stream)
        {
            SpriteFont font;

            switch (stream)
            {
                case FileStream fs when fs.Name.EndsWith("fnt"):
                {
                    using var reader = new StreamReader(stream);
                    var fontData = reader.ReadToEnd();

                    font = BMFontLoader.LoadXml(fontData,
                        name => new TextureWithOffset(_assetLoader.GetAsset<Texture2D>(name)));
                    break;
                }

                case FileStream fs when fs.Name.EndsWith("ttf"):
                {
                    using var memory = new MemoryStream();
                    stream.CopyTo(memory);
                    var bytes = memory.ToArray();

                    var bake = TtfFontBaker.Bake(
                        bytes,
                        48,
                        1024,
                        1024,
                        new[]
                        {
                            CharacterRange.BasicLatin,
                            CharacterRange.Latin1Supplement,
                            CharacterRange.LatinExtendedA,
                            CharacterRange.LatinExtendedB
                        });
                    font = bake.CreateSpriteFont(_graphicsDevice);
                    break;
                }

                default:
                    throw new InvalidDataException(nameof(stream));
            }

            return font;
        }
    }
}