using System;
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
        private readonly VirtualResolution _virtualResolution;
        private readonly GameWindow _window;

        private RenderTarget2D _mainRenderTarget;
        private RenderTarget2D _tempRenderTarget;

        public Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, VirtualResolution virtualResolution, GameWindow window)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _virtualResolution = virtualResolution;
            _window = window;
            _filters = new List<IFilter>();

            CreateBuffers();
        }

        private void CreateBuffers()
        {
            _mainRenderTarget = CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _tempRenderTarget = CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
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

            var rect = ScaledBackbuffer(out var scale);
            var width = (int) (rect.Width * scale);
            var height = (int) (rect.Height * scale);

            _spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(copy, 
                new Rectangle((_window.ClientBounds.Width / 2) - (width / 2), (_window.ClientBounds.Height / 2) - (height / 2), width, height), 
                rect, Color.White);
            _spriteBatch.End();
        }

        private Rectangle ScaledBackbuffer(out float scale)
        {
            var width = (float)_window.ClientBounds.Width;
            var height = (float)_window.ClientBounds.Height;
            var widthScale = width / (float)_virtualResolution.MinWidth;
            var heightScale = height / (float)_virtualResolution.MinHeight;
            var smallest = Math.Min(widthScale, heightScale);
            float trueWidth, trueHeight;

            if (widthScale < heightScale)
            {
                trueWidth = _virtualResolution.MinWidth;
                trueHeight = (int)(height / widthScale);
            }
            else
            {
                trueHeight = _virtualResolution.MinHeight;
                trueWidth = (int)(width / heightScale);
            }

            trueWidth = Math.Min(trueWidth, _virtualResolution.MaxWidth);
            trueHeight = Math.Min(trueHeight, _virtualResolution.MaxHeight);

            scale = smallest;

            return new Rectangle(
                (int)((_virtualResolution.MaxWidth / 2) - (trueWidth / 2)),
                (int)((_virtualResolution.MaxHeight / 2) - (trueHeight / 2)),
                (int)(trueWidth),
                (int)(trueHeight));
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
    }
}