using System;
using System.Collections.Generic;
using System.Text;
using PhotoVs.Engine.Assets;

namespace PhotoVs.Logic
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        string PaletteShader { get; }

        IStreamProvider StreamProvider { get; set; }
    }
}
