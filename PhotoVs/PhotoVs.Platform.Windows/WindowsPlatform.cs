using System;
using System.Collections.Generic;
using System.Text;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Desktop
{
    public class WindowsPlatform : IPlatform
    {
        public string PaletteShader => "shaders/color.dx11";
    }
}
