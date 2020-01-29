using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class EffectTypeLoader : ITypeLoader<Effect>
    {
        private readonly GraphicsDevice _graphicsDevice;

        public EffectTypeLoader(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public Effect Load(Stream stream)
        {
            using var reader = new BinaryReader(stream);
            return new Effect(_graphicsDevice, reader.ReadBytes((int) reader.BaseStream.Length));
        }
    }
}