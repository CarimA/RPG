using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoVs.Engine.Assets.StreamProviders
{
    public class FileSystemStreamProvider : IStreamProvider
    {
        public string ContentDirectory { get; }
        public string StorageDirectory { get; }

        public FileSystemStreamProvider(string contentDirectory, string storageDirectory)
        {
            ContentDirectory = FixDirectory(contentDirectory);
            StorageDirectory = FixDirectory(storageDirectory);

            CreateDirectory(DataLocation.Storage, "");
            CreateDirectory(DataLocation.Storage, "\\Saves\\");
            CreateDirectory(DataLocation.Storage, "\\Saves\\Save1\\");
            CreateDirectory(DataLocation.Storage, "\\Saves\\Save2\\");
            CreateDirectory(DataLocation.Storage, "\\Saves\\Save3\\");
            CreateDirectory(DataLocation.Storage, "\\Mods\\");
            CreateDirectory(DataLocation.Storage, "\\Screenshots\\");
            CreateDirectory(DataLocation.Storage, "\\Logs\\");
        }

        public void Write(DataLocation location, string filepath, Stream stream)
        {
            if (location == DataLocation.Content || location == DataLocation.Raw)
                throw new NotSupportedException("Cannot write files outside of storage.");

            var path = GetFilepath(location, filepath);
            using var fileStream = new FileStream(path, FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            fileStream.Write(bytes, 0, bytes.Length);
        }

        public Stream Read(DataLocation location, string filepath)
        {
            var path = GetFilepath(location, filepath);
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public void Delete(DataLocation location, string filepath)
        {
            if (location == DataLocation.Content || location == DataLocation.Raw)
                throw new NotSupportedException("Cannot delete files outside of storage.");

            var path = GetFilepath(location, filepath);
            File.Delete(path);
        }

        public void Copy(DataLocation location, string oldFilepath, string newFilepath)
        {
            if (location == DataLocation.Content || location == DataLocation.Raw)
                throw new NotSupportedException("Cannot copy files outside of storage.");

            var oldPath = GetFilepath(location, oldFilepath);
            var newPath = GetFilepath(location, newFilepath);
            File.Copy(oldPath, newPath);
        }

        public void Move(DataLocation location, string oldFilepath, string newFilepath)
        {
            if (location == DataLocation.Content || location == DataLocation.Raw)
                throw new NotSupportedException("Cannot move files outside of storage.");

            var oldPath = GetFilepath(location, oldFilepath);
            var newPath = GetFilepath(location, newFilepath);
            File.Move(oldPath, newPath);
        }

        public bool Exists(DataLocation location, string filepath)
        {
            var path = GetFilepath(location, filepath);
            return File.Exists(path);
        }

        public void CreateDirectory(DataLocation location, string directory)
        {
            var path = FixDirectory(GetFilepath(location, directory));
            Directory.CreateDirectory(path);
        }

        public IEnumerable<string> EnumerateFiles(DataLocation location, string directory)
        {
            var path = FixDirectory(GetFilepath(location, directory));
            var results = Directory.GetFiles(path, "*")
                .ToList()
                .Select(dir => dir.Substring(ResolveDirectory(location).Length));
            return results;
        }

        public IEnumerable<string> EnumerateDirectories(DataLocation location, string directory)
        {
            var path = FixDirectory(GetFilepath(location, directory));
            var results = Directory.GetDirectories(path + directory)
                .ToList()
                .Select(dir => dir.Substring(ResolveDirectory(location).Length));
            return results;
        }

        private string ResolveDirectory(DataLocation location)
        {
            return location switch
            {
                DataLocation.Content => ContentDirectory,
                DataLocation.Storage => StorageDirectory,
                DataLocation.Raw => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }

        private string FixFilename(string filename)
        {
            filename = filename.Replace("/", "\\");
            filename = filename.ToLowerInvariant();

            if (filename.StartsWith("\\"))
                filename = filename.Substring("\\".Length);

            return filename;
        }

        private string FixDirectory(string directory)
        {
            directory = FixFilename(directory);

            if (!directory.EndsWith("\\"))
                directory += "\\";

            return directory;
        }

        public string GetFilepath(DataLocation location, string filepath)
        {
            return location switch
            {
                DataLocation.Content => ContentDirectory + FixFilename(filepath),
                DataLocation.Storage => StorageDirectory + FixFilename(filepath),
                DataLocation.Raw => FixFilename(filepath),
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }
    }
}