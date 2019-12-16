using Microsoft.Xna.Framework;

namespace PhotoVs.FSM.Scenes
{
    public interface ISceneManager
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}