﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PhotoVs.Engine.Assets;

namespace PhotoVs.Platform.Android
{
    class AndroidStreamProvider : IStreamProvider
    {
        public string RootDirectory { get; } = "";
        private readonly AssetManager _assetManager;

        public AndroidStreamProvider(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public Stream GetFile(string filepath)
        {
            filepath = filepath.Replace('\\', '/');
            return _assetManager.Open(RootDirectory + filepath);
        }

        public IEnumerable<string> GetFiles(string directory)
        {
            return _assetManager.List(directory)
                .ToList()
                .Where(asset => !GetFiles(asset).Any());
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            return _assetManager.List(directory)
                .ToList()
                .Where(asset => GetFiles(asset).Any());
        }
    }
}