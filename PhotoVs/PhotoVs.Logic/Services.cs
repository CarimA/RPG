using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic
{
    public class Services
    {
        public Events Events { get; private set; }

        public IAssetLoader AssetLoader { get; private set; }

        public Services(Events events)
        {
            Events = events;
        }

        public void SetAssetLoader(IAssetLoader assetLoader)
        {
            AssetLoader = assetLoader;
            Events.RaiseOnServiceSet(assetLoader);
        }
    }
}
