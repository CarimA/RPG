using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{
    public interface IFilter
    {
        void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture);
    }

    public interface ITransformFilter : IFilter
    {
        void SetTransform(Matrix matrix);
    }
}