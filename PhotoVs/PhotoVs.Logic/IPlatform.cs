using PhotoVs.Engine.Assets.StreamProviders;

namespace PhotoVs.Logic
{
    public interface IPlatform
    {
        bool OverrideFullscreen { get; }
        string PaletteShader { get; }
        string AverageShader { get; }

        IStreamProvider StreamProvider { get; set; }
    }
}
