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
        private readonly CanvasSize _targetCanvasSize;

        private RenderTarget2D _mainRenderTarget;
        private RenderTarget2D _tempRenderTarget;

        public Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, CanvasSize targetCanvasSize)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _targetCanvasSize = targetCanvasSize;
            _filters = new List<IFilter>();

            ResizeBuffers();
            _targetCanvasSize.OnResize += ResizeBuffers;
        }

        public RenderTarget2D Backbuffer => _mainRenderTarget;

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

                filter.Filter(ref copy, _spriteBatch, copy);
            }

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(copy, _targetCanvasSize.VirtualDisplay, Color.White);
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


        private void ResizeBuffers()
        {
            _mainRenderTarget?.Dispose();
            _tempRenderTarget?.Dispose();
            _mainRenderTarget = null;
            _tempRenderTarget = null;

            _mainRenderTarget = CreateRenderTarget(_targetCanvasSize.TrueCurrentWidth, _targetCanvasSize.TrueCurrentHeight);
            _tempRenderTarget = CreateRenderTarget(_targetCanvasSize.TrueCurrentWidth, _targetCanvasSize.TrueCurrentHeight);
        }
    }
}