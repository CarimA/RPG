using System.Collections.Generic;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Audio;

namespace PhotoVs.Engine
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        Dictionary<string, string> FileExtensionReplacement { get; }
        IAudio Audio { get; }
        IStreamProvider StreamProvider { get; set; }
    }
}