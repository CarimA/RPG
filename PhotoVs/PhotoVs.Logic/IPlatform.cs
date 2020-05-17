using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Logic
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        string PaletteShader { get; }

        IStreamProvider StreamProvider { get; set; }
    }
}
