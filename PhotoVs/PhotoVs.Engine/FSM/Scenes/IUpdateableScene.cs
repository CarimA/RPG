using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.FSM.Scenes
{
    public interface IUpdateableScene : IScene
    {
        void Update(GameTime gameTime);
    }
}