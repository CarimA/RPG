using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.FSM
{
    public interface IDrawableScene : IScene
    {
        void Draw(GameTime gameTime);
        void DrawUI(GameTime gameTime, Matrix uiOrigin);
    }
}