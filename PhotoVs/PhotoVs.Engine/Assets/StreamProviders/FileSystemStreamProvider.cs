using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoVs.Engine.Assets.StreamProviders
{
    public class FileSystemStreamProvider : IStreamProvider
    {
        public FileSystemStreamProvider(string directory)
        {
            RootDirectory = directory;
        }

        public string RootDirectory { get; set; }

        public IEnumerable<string> GetFiles(string directory, bool includeRoot = true)
        {
            var path = includeRoot ? RootDirectory + directory : directory;
            var results = Directory.GetFiles(path, "*")
                .ToList()
                .Select(dir => dir.Substring(RootDirectory.Length));
            return results;
        }

        public IEnumerable<string> GetDirectories(string directory, bool includeRoot = true)
        {
            var path = includeRoot ? RootDirectory + directory : directory;
            var results = Directory.GetDirectories(path + directory)
                .ToList()
                .Select(dir => dir.Substring(RootDirectory.Length));
            return results;
        }

        public Stream GetFile(string filepath, bool includeRoot = true)
        {
            var path = includeRoot ? RootDirectory + filepath : filepath;
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}