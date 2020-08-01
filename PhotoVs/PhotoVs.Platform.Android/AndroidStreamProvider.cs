using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content.Res;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Platform.Android
{
    internal class AndroidStreamProvider : IStreamProvider
    {
        private readonly AssetManager _assetManager;

        public AndroidStreamProvider(AssetManager assetManager)
        {
            _assetManager = assetManager;
            StorageDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            CreateDirectory(DataLocation.Storage, "");
            CreateDirectory(DataLocation.Storage, "\\Saves\\");
            CreateDirectory(DataLocation.Storage, "\\Saves\\Save1\\");
            CreateDirectory(DataLocation.Storage, "\\Saves\\Save2\\");
            CreateDirectory(DataLocation.Storage, "\\Saves\\Save3\\");
            CreateDirectory(DataLocation.Storage, "\\Mods\\");
            CreateDirectory(DataLocation.Storage, "\\Screenshots\\");
            CreateDirectory(DataLocation.Storage, "\\Logs\\");
        }

        public string ContentDirectory { get; }
        public string StorageDirectory { get; }

        public void Write(DataLocation location, string filepath, Stream stream)
        {
            if (location == DataLocation.Storage)
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                File.WriteAllBytes(StorageDirectory + location, ms.ToArray());
            }
        }

        public Stream Read(DataLocation location, string filepath)
        {
            if (location == DataLocation.Storage)
            {
                var bytes = File.ReadAllBytes(StorageDirectory + location);
                var ms = new MemoryStream(bytes);
                return ms;
            }

            if (location == DataLocation.Content)
                //var test = debugasset(_assetManager, "content").ToList(); //.List("/assets");
                return _assetManager.Open("content/" + filepath);

            throw new NotSupportedException();
        }

        public void Delete(DataLocation location, string filepath)
        {
            throw new NotImplementedException();
        }

        public void Copy(DataLocation location, string oldFilepath, string newFilepath)
        {
            throw new NotImplementedException();
        }

        public void Move(DataLocation location, string oldFilepath, string newFilepath)
        {
            throw new NotImplementedException();
        }

        public bool Exists(DataLocation location, string filepath)
        {
            throw new NotImplementedException();
        }

        public long LastModified(DataLocation location, string filepath)
        {
            throw new NotImplementedException();
        }

        public void CreateDirectory(DataLocation location, string directory)
        {
            if (location == DataLocation.Storage) Directory.CreateDirectory(StorageDirectory + "/" + directory);
        }

        public IEnumerable<string> EnumerateFiles(DataLocation location, string directory)
        {
            if (location == DataLocation.Content)
            {
                directory = Sanitise(directory);
                directory = RemoveTrailingSlash(directory);
                var results = _assetManager.List("content/" + directory);
                return results
                    .ToList()
                    .Select(asset => directory + "/" + asset)
                    .Where(IsFile);
            }

            if (location == DataLocation.Storage)
            {
                if (!Directory.Exists(StorageDirectory + "/" + directory))
                    Directory.CreateDirectory(StorageDirectory + "/" + directory);

                return Directory.EnumerateFiles(StorageDirectory + "/" + directory);
            }

            throw new NotSupportedException();
        }

        public IEnumerable<string> EnumerateDirectories(DataLocation location, string directory)
        {
            throw new NotImplementedException();
        }

        public string GetFilepath(DataLocation location, string filepath)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> debugasset(AssetManager asset, string folder)
        {
            var a = asset.List(folder);

            foreach (var s in a)
            {
                yield return s;
                debugasset(asset, s);
            }
        }

        private bool IsFile(string filepath)
        {
            var result = !IsDirectory(filepath);
            return result;
        }

        private bool IsDirectory(string filepath)
        {
            var result = _assetManager.List(filepath);
            return result.Length > 0;
        }

        private string Sanitise(string input)
        {
            return input.Replace('\\', '/');
        }

        private string RemoveTrailingSlash(string input)
        {
            if (input.Last() == '/') return input.Substring(0, input.LastIndexOf('/'));

            return input;
        }
    }
}