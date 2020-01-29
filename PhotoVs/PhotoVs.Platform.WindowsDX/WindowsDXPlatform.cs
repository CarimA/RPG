using System;
using System.Collections.Generic;
using System.Text;
using PhotoVs.Logic;

namespace PhotoVs.Platform.WindowsDX
{
    public class WindowsDXPlatform : IPlatform
    {
        public string PaletteShader => "shaders/color.dx11";
    }
}
