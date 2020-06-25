using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Engine
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        string ShaderFileExtension { get; }
        IStreamProvider StreamProvider { get; set; }
    }
}
