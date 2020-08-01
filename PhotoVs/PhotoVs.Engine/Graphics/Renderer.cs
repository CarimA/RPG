using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Graphics.Filters;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer : IRenderer, IHasBeforeDraw, IHasAfterDraw
    {
        private readonly List<IFilter> _filters;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly ICanvasSize _targetCanvasSize;

        private RenderTarget2D _mainRenderTarget;
        private RenderTarget2D _tempRenderTarget;

        public Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ICanvasSize targetCanvasSize)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _targetCanvasSize = targetCanvasSize;
            _filters = new List<IFilter>();

            ResizeBuffers();
            _targetCanvasSize.OnResize += ResizeBuffers;
        }

        public int AfterDrawPriority { get; set; } = int.MaxValue;
        public bool AfterDrawEnabled { get; set; } = true;

        public int BeforeDrawPriority { get; set; } = int.MinValue;
        public bool BeforeDrawEnabled { get; set; } = true;

        public void AddFilter(IFilter filter)
        {
            _filters.Add(filter);
        }

        public void BeforeDraw(GameTime gameTime)
        {
            _graphicsDevice.SetRenderTarget(_mainRenderTarget);
            _graphicsDevice.Clear(Color.CornflowerBlue);
        }

        public void AfterDraw(GameTime gameTime)
        {
            var copy = _mainRenderTarget;
            foreach (var filter in _filters)
            {
                if (filter is IUpdateFilter updateFilter)
                    updateFilter.Update(gameTime);

                copy = filter.Filter(_spriteBatch, copy);
            }

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(copy, _targetCanvasSize.DisplayRectangle, Color.White);
            _spriteBatch.End();
        }

        public void RequestSubRenderer(RenderTarget2D renderTarget)
        {
            // copy the existing buffer to a temporary buffer so
            // that it isn't wiped
            _graphicsDevice.SetRenderTarget(_tempRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_mainRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _graphicsDevice.SetRenderTarget(renderTarget);
            _graphicsDevice.Clear(Color.Transparent);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return new RenderTarget2D(_graphicsDevice, width, height);
        }

        public void RelinquishSubRenderer()
        {
            _graphicsDevice.SetRenderTarget(_mainRenderTarget);
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_tempRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        public Matrix GetUIOrigin()
        {
            return Matrix.CreateTranslation(new Vector3(
                _targetCanvasSize.DisplayWidth / 2 - _targetCanvasSize.Width / 2,
                _targetCanvasSize.DisplayHeight / 2 - _targetCanvasSize.Height / 2, 0));
        }

        private void ResizeBuffers()
        {
            _mainRenderTarget = CreateRenderTarget(_targetCanvasSize.DisplayWidth, _targetCanvasSize.DisplayHeight);
            _tempRenderTarget = CreateRenderTarget(_targetCanvasSize.DisplayWidth, _targetCanvasSize.DisplayHeight);
        }
    }
}