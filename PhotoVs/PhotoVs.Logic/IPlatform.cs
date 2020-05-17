using System;
using System.Collections.Generic;
using System.Text;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Logic
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        string PaletteShader { get; }

        IStreamProvider StreamProvider { get; set; }
    }
}
