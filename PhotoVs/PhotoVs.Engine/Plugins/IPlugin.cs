namespace PhotoVs.Engine.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        int Version { get; }
    }
}