using Microsoft.Xna.Framework;

namespace PhotoVs.Logic.Scenes.Transitions
{
    public interface ITransition
    {
        bool IsFinished { get; }

        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        bool ShouldSwitch();
    }
}