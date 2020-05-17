using Android.Content.Res;
using PhotoVs.Engine.Assets.StreamProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            return _assetManager.Open(RootDirectory + filepath);
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
    }
}