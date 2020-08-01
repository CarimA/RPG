namespace PhotoVs.Logic.Mechanics.World
{
    public interface IOverworld
    {
        void LoadMaps(string directory);
        void SetMap(string map);
        OverworldMap GetMap();
    }
}