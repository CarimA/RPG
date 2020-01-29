using System;
using System.Collections.Generic;
using System.Text;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Desktop
{
    public class DesktopPlatform : IPlatform
    {
        public string PaletteShader => "shaders/color.ogl";
        public IStreamProvider StreamProvider { get; set; }

        public DesktopPlatform()
        {
            StreamProvider = new FileSystemStreamProvider("assets/");
        }
    }
}
