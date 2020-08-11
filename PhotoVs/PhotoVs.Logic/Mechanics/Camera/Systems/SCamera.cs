using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Utils;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.Camera.Systems
{
    public class SCamera : IUpdateableSystem
    {
        private readonly ICanvasSize _canvasSize;
        private readonly Random _random;

        private bool _isDirty;
        private Vector2 _lastPosition;

        private Vector2 _lerpPosition;
        private float _lerpZoom;
        private Vector2 _position;
        private float _rotate;
        private GameObject _target;

        public SCamera(ICanvasSize canvasSize)
        {
            _canvasSize = canvasSize;
            _isDirty = false;
            Zoom = 1f;
            _rotate = 0f;
            _random = new Random();
        }

        public Matrix Transform { get; private set; } = Matrix.Identity;

        public float CurrentZoom => _lerpZoom;
        public float Zoom { get; private set; }

        public int Priority { get; set; } = -99;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { };

        public void BeforeUpdate(GameTime gameTime)
        {
            CheckPositionChanged();
            UpdateLerp(gameTime);

            if (_isDirty)
                UpdateCamera();
        }

        public void Update(GameTime gameTime, GameObjectList entities)
        {
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void CheckPositionChanged()
        {
            var hasPosition = _target.Components.TryGet(out CPosition position);
            var hasSize = _target.Components.TryGet(out CSize size);

            if (!hasPosition)
                return;

            if (!hasSize)
                if (_position == position.Position)
                    return;

            _lastPosition = _position;
            _position = position.Position;

            // todo: maybe rename to COffsetCamera?
            if (hasSize)
                _position += size.Size / 2;

            _isDirty = true;
        }

        private void UpdateLerp(GameTime gameTime)
        {
            if (Math.Abs(Zoom - _lerpZoom) > 0f)
            {
                _lerpZoom = MathHelper.Lerp(_lerpZoom, Zoom, 1f * gameTime.GetElapsedSeconds());
                _isDirty = true;
            }

            //int lookDistance = 120;
            if (Vector2.Distance(_position, _lerpPosition) > 0.5)
            {
                _lerpPosition = Vector2.Lerp(_lerpPosition, _position, 5f * gameTime.GetElapsedSeconds());
                _isDirty = true;
            }
        }

        public void Set(List<Vector2> points)
        {
            // this method will set a zoom based on the
            // given points in world view that want to be 
            // seen in view
            var rect = new RectangleF(points.First().X,
                points.First().Y,
                1, 1);

            // find a bounding rectangle based on all of the points
            foreach (var point in points)
            {
                if (point.X < rect.Left)
                    rect.X = point.X;

                if (point.X > rect.Right)
                    rect.Width = point.X - rect.X;

                if (point.Y < rect.Top)
                    rect.Y = point.Y;

                if (point.Y > rect.Bottom)
                    rect.Height = point.Y - rect.Y;
            }

            var midpoint = rect.Center;
            var distance = Vector2.Distance(rect.TopLeft, rect.BottomRight);

            var zWidth = _canvasSize.DisplayWidth / rect.Width;
            var zHeight = _canvasSize.DisplayHeight / rect.Height;

            var zoom = Math.Min(zWidth, zHeight);

            var follow = new GameObject();
            follow.Components.Add(new CPosition(midpoint));
            Follow(follow);
            Zoom = zoom;
        }

        private void UpdateCamera()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-_lerpPosition.X, -_lerpPosition.Y, 0)) *
                        Matrix.CreateScale(new Vector3(_lerpZoom, _lerpZoom, 1)) *
                        Matrix.CreateTranslation(new Vector3(_canvasSize.DisplayWidth / 2f,
                            _canvasSize.DisplayHeight / 2f,
                            0)) *
                        Matrix.CreateRotationZ(_rotate);
            _isDirty = false;
        }

        public void SetZoom(float zoom)
        {
            if (Zoom == zoom)
                return;

            var max = 10f;
            var min = 0.000000001f;
            Zoom = Math.Min(max, Math.Max(min, zoom));
            _isDirty = true;
        }

        public void SetRotate(float rotate)
        {
            if (_rotate == rotate)
                return;

            _rotate = rotate % (MathHelper.Pi * 2);
            _isDirty = true;
        }

        public void Follow(GameObject target)
        {
            if (_target == target)
                return;

            var hasPosition = target.Components.TryGet(out CPosition position);
            var hasSize = target.Components.TryGet(out CSize size);

            if (!hasPosition)
                return;

            _target = target;
            _lastPosition = position.Position;

            if (hasSize)
                _lastPosition += size.Size / 2;

            _isDirty = true;
            Zoom = 1f;
        }

        public Vector2 WorldToScreen(Vector2 position)
        {
            return Vector2.Transform(position, Transform);
        }

        public Vector2 ScreenToWorld(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(Transform));
        }

        public Rectangle VisibleArea()
        {
            if (Transform == Matrix.Identity)
                return Rectangle.Empty;

            var inverseTransform = Matrix.Invert(Transform);

            var topLeft = Vector2.Transform(Vector2.Zero, inverseTransform);
            var topRight = Vector2.Transform(new Vector2(_canvasSize.DisplayWidth, 0), inverseTransform);
            var bottomLeft = Vector2.Transform(new Vector2(0, _canvasSize.DisplayHeight), inverseTransform);
            var bottomRight = Vector2.Transform(
                new Vector2(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight),
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