using Microsoft.Xna.Framework;

namespace PhotoVs.Models.FSM
{
    public interface IUpdateableScene : IScene
    {
        void Update(GameTime gameTime);
    }
}