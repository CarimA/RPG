using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{
    public interface IFilter
    {
        RenderTarget2D Filter(SpriteBatch spriteBatch, Texture2D inputTexture);
    }
}
