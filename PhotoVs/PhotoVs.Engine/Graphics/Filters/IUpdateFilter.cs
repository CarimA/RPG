using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Graphics.Filters
{
    public interface IUpdateFilter : IFilter
    {
        void Update(GameTime gameTime);
    }
}
