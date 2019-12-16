using Microsoft.Xna.Framework;

namespace PhotoVs.FSM.Scenes
{
    public interface IUpdateableScene : IScene
    {
        void Update(GameTime gameTime);
    }
}