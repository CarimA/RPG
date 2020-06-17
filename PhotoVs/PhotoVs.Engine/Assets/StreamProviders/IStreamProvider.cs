using System.Collections.Generic;
using System.IO;

namespace PhotoVs.Engine.Assets.StreamProviders
{
    public interface IStreamProvider
    {
        string ContentDirectory { get; }
        string StorageDirectory { get; }

        void Write(DataLocation location, string filepath, Stream stream);
        Stream Read(DataLocation location, string filepath);
        void Delete(DataLocation location, string filepath);
        void Copy(DataLocation location, string oldFilepath, string newFilepath);
        void Move(DataLocation location, string oldFilepath, string newFilepath);
        bool Exists(DataLocation location, string filepath);

        void CreateDirectory(DataLocation location, string directory);

        IEnumerable<string> EnumerateFiles(DataLocation location, string directory);
        IEnumerable<string> EnumerateDirectories(DataLocation location, string directory);

        string GetFilepath(DataLocation location, string filepath);
    }
}