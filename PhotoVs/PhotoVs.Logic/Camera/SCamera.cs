using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Transforms;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Camera
{
    public class SCamera : IUpdateableSystem
    {
        private readonly Random _random;
        private readonly Renderer _renderer;
        private readonly List<ScreenShake> _shakes;

        private bool _isDirty;
        private Vector2 _lastPosition;

        private Vector2 _lerpPosition;
        private Vector2 _lookDirection;
        private float _permanentShake;
        private Vector2 _position;
        private float _rotate;
        private IGameObject _target;

        private Matrix _transform = Matrix.Identity;
        private float _zoom;

        public SCamera(Renderer renderer)
        {
            _renderer = renderer;
            _isDirty = true;
            _zoom = 1f;
            _rotate = 0f;
            _shakes = new List<ScreenShake>();
            _random = new Random();
        }

        public int Priority { get; set; } = -99;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { };

        public void BeforeUpdate(GameTime gameTime)
        {
            UpdateShakes(gameTime);
            CheckPositionChanged();
            UpdateLerp(gameTime);
            UpdateLookDirection();

            if (_isDirty || _shakes.Count > 0)
                UpdateCamera();
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void UpdateShakes(GameTime gameTime)
        {
            _shakes.ForEach(UpdateShake(gameTime));
            _shakes.RemoveAll(RemoveDeadShake);
        }

        private Action<ScreenShake> UpdateShake(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            return tuple => tuple.Duration -= dt;
        }


        private bool RemoveDeadShake(ScreenShake shake)
        {
            return shake.Duration <= 0f;
        }

        private float ShakeIntensity()
        {
            return _shakes.Sum(ShakeSum) + _permanentShake;
        }

        private float ShakeSum(ScreenShake tuple)
        {
            return tuple.GetIntensity();
        }

        private void CheckPositionChanged()
        {
            var hasPosition = _target.Components.TryGet(out CPosition position);
            var hasSize = _target.Components.TryGet(out CSize size);

            if (!hasPosition)
                return;

            if (!hasSize)
            {
                if (_position == position.Position)
                    return;
            }
            else
            {
                if (_position == position.Position + (size.Size / 2))
                    return;
            }

            _lastPosition = _position;
            _position = position.Position;

            // todo: maybe rename to COffsetCamera?
            if (hasSize)
                _position += size.Size / 2;

            _isDirty = true;
        }

        private void UpdateLerp(GameTime gameTime)
        {
            if (Vector2.Distance(_position + _lookDirection, _lerpPosition) < 1)
                return;

            _lerpPosition = Vector2.Lerp(_lerpPosition, _position + _lookDirection, 0.25f);
            _isDirty = true;
        }

        private void UpdateLookDirection()
        {
            var lastLook = _lookDirection;
            _lookDirection = (_position - _lastPosition) * 8;

            if (Vector2.Distance(lastLook, _lookDirection) < 1)
                return;

            _isDirty = true;
        }

        private void UpdateCamera()
        {
            var intensity = ShakeIntensity();
            _transform = Matrix.CreateTranslation(new Vector3(-_lerpPosition.X, -_lerpPosition.Y, 0)) *
                         Matrix.CreateTranslation(new Vector3(
                             -((float) (_random.NextDouble() * intensity * 2) - intensity),
                             -((float) (_random.NextDouble() * intensity * 2) - intensity), 0)) *
                         Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
                         Matrix.CreateTranslation(new Vector3(_renderer.GetCanvasSize().GetWidth() / 2,
                             _renderer.GetCanvasSize().GetHeight() / 2,
                             0)) *
                         Matrix.CreateRotationZ(_rotate);
            _isDirty = false;
        }

        public void Attach(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _transform);
        }

        public void Detach(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }

        public void SetZoom(float zoom)
        {
            if (_zoom == zoom)
                return;

            var max = 4f;
            var min = 0.5f;
            _zoom = Math.Min(max, Math.Max(min, zoom));
            _isDirty = true;
        }

        public void SetRotate(float rotate)
        {
            if (_rotate == rotate)
                return;

            _rotate = rotate % (MathHelper.Pi * 2);
            _isDirty = true;
        }

        public void Shake(float intensity, float duration)
        {
            _shakes.Add(new ScreenShake(intensity, duration));
        }

        public void Shake(float intensity)
        {
            _permanentShake = intensity;
        }

        public void Follow(IGameObject target)
        {
            if (_target == target)
                return;

            if (!target.Components.Has<CPosition>())
                return;

            _target = target;
            _isDirty = true;
        }

        public Vector2 WorldToScreen(Vector2 position)
        {
            return Vector2.Transform(position, _transform);
        }

        public Vector2 ScreenToWorld(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(_transform));
        }

        public Rectangle VisibleArea()
        {
            if (_transform == Matrix.Identity)
                return Rectangle.Empty;

            var inverseTransform = Matrix.Invert(_transform);

            var topLeft = Vector2.Transform(Vector2.Zero, inverseTransform);
            var topRight = Vector2.Transform(new Vector2(_renderer.GetCanvasSize().GetWidth(), 0), inverseTransform);
            var bottomLeft = Vector2.Transform(new Vector2(0, _renderer.GetCanvasSize().GetHeight()), inverseTransform);
            var bottomRight = Vector2.Transform(
                new Vector2(_renderer.GetCanvasSize().GetWidth(), _renderer.GetCanvasSize().GetHeight()),
                inverseTransform);

            var min = new Vector2(
                MathHelper.Min(topLeft.X, MathHelper.Min(topRight.X, MathHelper.Min(bottomLeft.X, bottomRight.X))),
                MathHelper.Min(topLeft.Y, MathHelper.Min(topRight.Y, MathHelper.Min(bottomLeft.Y, bottomRight.Y))));
            var max = new Vector2(
                MathHelper.Max(topLeft.X, MathHelper.Max(topRight.X, MathHelper.Max(bottomLeft.X, bottomRight.X))),
                MathHelper.Max(topLeft.Y, MathHelper.Max(topRight.Y, MathHelper.Max(bottomLeft.Y, bottomRight.Y))));

            var bounds = new Rectangle((int) min.X, (int) min.Y, (int) (max.X - min.X), (int) (max.Y - min.Y));
            return bounds;
        }

        public bool IsVisible(Rectangle bounds)
        {
            var visibleArea = VisibleArea();
            return visibleArea.Contains(bounds) || visibleArea.Intersects(bounds);
        }
    }
}