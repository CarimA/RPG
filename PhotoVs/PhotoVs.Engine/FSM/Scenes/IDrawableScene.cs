using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.FSM.Scenes
{
    public interface IDrawableScene : IScene
    {
        void Draw(GameTime gameTime);
        void DrawUI(GameTime gameTime, Matrix uiOrigin);
    }
}