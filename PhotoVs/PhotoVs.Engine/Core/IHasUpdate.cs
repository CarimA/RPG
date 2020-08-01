using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IHasUpdate
    {
        int UpdatePriority { get; set; }
        bool UpdateEnabled { get; set; }
        void Update(GameTime gameTime);
    }
}