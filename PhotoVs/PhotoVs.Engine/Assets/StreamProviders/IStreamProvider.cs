﻿using System.Collections.Generic;
using System.IO;

namespace PhotoVs.Engine.Assets
{
    public interface IStreamProvider
    {
        string RootDirectory { get; }
        Stream GetFile(string filepath);
        IEnumerable<string> GetFiles(string directory);
        IEnumerable<string> GetDirectories(string directory);
    }
}