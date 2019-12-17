using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoVs.Models.Assets;

namespace PhotoVs.Engine.Assets.StreamProviders
{
    public class FileSystemStreamProvider : IStreamProvider
    {
        public FileSystemStreamProvider(string directory)
        {
            RootDirectory = directory;
        }

        public string RootDirectory { get; set; }

        public IEnumerable<string> GetFiles(string directory)
        {
            return Directory.GetFiles(RootDirectory + directory, "*").ToList()
                .Select(RemoveRootDirectory);
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            return Directory.GetDirectories(RootDirectory + directory).ToList()
                .Select(RemoveRootDirectory);
        }

        public Stream GetFile(string filepath)
        {
            return new FileStream(RootDirectory + filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private string RemoveRootDirectory(string input)
        {
            return input.Substring(RootDirectory.Length);
        }
    }
}