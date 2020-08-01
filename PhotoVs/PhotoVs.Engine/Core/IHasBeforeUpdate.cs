using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IHasBeforeUpdate
    {
        int BeforeUpdatePriority { get; set; }
        bool BeforeUpdateEnabled { get; set; }
        void BeforeUpdate(GameTime gameTime);
    }
}