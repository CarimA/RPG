﻿using System;
using System.Collections.Generic;
using System.Text;
using Android.Content.Res;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Android
{
    public class AndroidPlatform : IPlatform
    {
        public bool OverrideFullscreen => true;
        public string PaletteShader => "shaders/color.ogl";
        public IStreamProvider StreamProvider { get; set; }

        public AndroidPlatform(AssetManager assetManager)
        {
            StreamProvider = new AndroidStreamProvider(assetManager);
        }
    }
}
