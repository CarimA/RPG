using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic
{
    public interface IGameState
    {
        Config Config { get; }
        Player Player { get; }
        SCamera Camera { get; }
        GameObjectList GameObjects { get; }
        SystemList Systems { get; }
        GameTime GameTime { get; set; }
    }
}