using System.IO;
using Microsoft.Xna.Framework.Graphics;

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
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var bytes = ms.ToArray();
            return new Effect(_graphicsDevice, bytes);

            /*using var reader = new BinaryReader(stream);
            var length = (int)reader.BaseStream.Length;
            var bytes = reader.ReadBytes(length);
            return new Effect(_graphicsDevice, bytes);*/
        }
    }
}