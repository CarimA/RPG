using System.Collections.Generic;
using PhotoVs.Engine.Assets;

namespace PhotoVs.Engine.Scripting
{
    public class ScriptData
    {
        public ScriptData(List<(DataLocation, string)> directories)
        {
            Directories = directories;
        }

        public List<(DataLocation, string)> Directories { get; }
    }
}