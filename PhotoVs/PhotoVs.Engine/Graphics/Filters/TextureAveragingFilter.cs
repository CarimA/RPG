using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class TextureAveragingFilter
    {
        private readonly Effect _effect;
        private readonly Renderer _renderer;

        public TextureAveragingFilter(Renderer renderer, Effect effect)
        {
            _renderer = renderer;
            _effect = effect;
        }

        public void Filter(SpriteBatch spriteBatch, Texture2D inputTexture)
        {
        }
    }
}