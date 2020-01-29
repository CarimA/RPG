using LiteDB;
using PhotoVs.Engine.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoVs.Engine.Assets.StreamProviders
{
    class LiteDBStreamProvider : IStreamProvider, IDisposable
    {
        private readonly LiteDatabase _database;

        public string RootDirectory { get; set; }

        public LiteDBStreamProvider(string filepath)
        {
            _database = new LiteDatabase(filepath);
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            throw new NotImplementedException();
        }

        public Stream GetFile(string filepath)
        {
            return _database.FileStorage.OpenRead(filepath);
        }

        public IEnumerable<string> GetFiles(string directory)
        {
            return _database.FileStorage.Find(directory)
                .Select(file => file.Id);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
