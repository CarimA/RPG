using Android.Content.Res;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.StreamProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoVs.Platform.Android
{
    class AndroidStreamProvider : IStreamProvider
    {
        private readonly AssetManager _assetManager;

        public AndroidStreamProvider(AssetManager assetManager)
        {
            _assetManager = assetManager;
            StorageDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }


        public IEnumerable<string> GetFiles(string directory)
        {
            directory = Sanitise(directory);
            directory = RemoveTrailingSlash(RootDirectory + directory);
            var results = _assetManager.List(directory);
            return results
                .ToList()
                .Select(asset => directory + "/" + asset)
                .Where(IsFile);
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            directory = Sanitise(directory);
            directory = RemoveTrailingSlash(RootDirectory + directory);
            var list = _assetManager.List(RootDirectory + directory).ToList();
            var results = list
                .Select(asset => directory + "/" + asset)
                .Where(IsDirectory);
            return results;
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
            if (input.Last() == '/')
            {
                return input.Substring(0, input.LastIndexOf('/'));
            }

            return input;
        }

        public string RootDirectory { get; }
        public string ContentDirectory { get; }
        public string StorageDirectory { get; }
        public void Write(DataLocation location, string filepath, Stream stream)
        {
            throw new NotImplementedException();
        }

        public Stream Read(DataLocation location, string filepath)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<string> EnumerateFiles(DataLocation location, string directory)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> EnumerateDirectories(DataLocation location, string directory)
        {
            throw new NotImplementedException();
        }

        public string GetFilepath(DataLocation location, string filepath)
        {
            throw new NotImplementedException();
        }
    }
}