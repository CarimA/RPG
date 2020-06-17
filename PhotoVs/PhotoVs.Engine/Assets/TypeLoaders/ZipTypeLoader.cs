using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Engine.Assets.TypeLoaders
{
    public class ZipTypeLoader : ITypeLoader<ZipStorer>
    {
        public ZipStorer Load(Stream stream)
        {
            var zip = ZipStorer.Open(stream, FileAccess.Read);
            return zip;
        }
    }
}
