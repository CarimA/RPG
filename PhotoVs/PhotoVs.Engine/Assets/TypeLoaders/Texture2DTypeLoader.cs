using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class Texture2DTypeLoader : TypeLoader<Texture2D>
    {
        private readonly GraphicsDevice _graphicsDevice;

        public Texture2DTypeLoader(IAssetLoader assetLoader, GraphicsDevice graphicsDevice) : base(assetLoader)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override Texture2D Load(Stream stream)
        {
            return Texture2D.FromStream(_graphicsDevice, stream);
        }
    }
}