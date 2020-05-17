﻿using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class Texture2DTypeLoader : ITypeLoader<Texture2D>
    {
        private readonly GraphicsDevice _graphicsDevice;

        public Texture2DTypeLoader(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public Texture2D Load(Stream stream)
        {
            return Texture2D.FromStream(_graphicsDevice, stream);
        }
    }
}