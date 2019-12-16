using Microsoft.Xna.Framework;

namespace PhotoVs.FSM.Scenes
{
    public interface IDrawableScene : IScene
    {
        void Draw(GameTime gameTime);
    }
}