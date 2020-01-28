using System;
using System.Collections.Generic;
using System.Text;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Desktop
{
    public class DesktopPlatform : IPlatform
    {
        public string PaletteShader => "shaders/color.ogl";
    }
}
