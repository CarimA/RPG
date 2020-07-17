using System.Collections.Generic;
using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Engine
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        Dictionary<string, string> FileExtensionReplacement { get; }

        IStreamProvider StreamProvider { get; set; }

    }
}
