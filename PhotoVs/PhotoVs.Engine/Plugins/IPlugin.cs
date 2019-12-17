namespace PhotoVs.Engine.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }

        void Bind(Events gameEvents);
    }
}