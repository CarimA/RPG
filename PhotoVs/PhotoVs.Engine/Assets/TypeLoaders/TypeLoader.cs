using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.Assets.AssetLoaders;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public abstract class TypeLoader<T> : ITypeLoader<T>
    {
        private IAssetLoader _assetLoader;

        public TypeLoader(IAssetLoader assetLoader)
        {
            assetLoader.RegisterTypeLoader<T>(this);
        }

        public abstract T Load(Stream stream);
    }
}
