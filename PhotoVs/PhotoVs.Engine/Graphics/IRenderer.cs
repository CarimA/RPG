using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics.Filters;

namespace PhotoVs.Engine.Graphics
{
    public interface IRenderer
    {
        void AddFilter(IFilter filter);
        void BeforeDraw(GameTime gameTime);
        void AfterDraw(GameTime gameTime);
        void RequestSubRenderer(RenderTarget2D renderTarget);
        RenderTarget2D CreateRenderTarget(int width, int height);
        void RelinquishSubRenderer();
        RenderTarget2D Backbuffer { get; }
    }
}