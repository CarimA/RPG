using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.FSM.Scenes
{
    public interface ISceneManager
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void DrawUI(GameTime gameTime, Matrix uiOrigin);
    }
}