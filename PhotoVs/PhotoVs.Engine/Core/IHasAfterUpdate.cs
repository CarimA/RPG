using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IHasAfterUpdate
    {
        int AfterUpdatePriority { get; set; }
        bool AfterUpdateEnabled { get; set; }
        void AfterUpdate(GameTime gameTime);
    }
}