using System.Collections.Generic;
using System.IO;

namespace PhotoVs.Engine.Assets.StreamProviders
{
    public interface IStreamProvider
    {
        string RootDirectory { get; }
        Stream GetFile(string filepath, bool includeRoot = true);
        IEnumerable<string> GetFiles(string directory, bool includeRoot = true);
        IEnumerable<string> GetDirectories(string directory, bool includeRoot = true);
    }
}