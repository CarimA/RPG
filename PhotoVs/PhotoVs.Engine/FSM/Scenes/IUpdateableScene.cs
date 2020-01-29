using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.FSM
{
    public interface IUpdateableScene : IScene
    {
        void Update(GameTime gameTime);
    }
}