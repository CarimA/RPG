using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Utils;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics
{
    public class Camera
    {
        private readonly VirtualResolution _virtualResolution;
        private readonly SpriteBatch _spriteBatch;
        private readonly Primitive _primitive;
        private readonly GraphicsDeviceManager _graphics;

        private bool _isDirty;

        private float _zoom;
        private float _inverseZoom;

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (Math.Abs(Zoom - value) < 0.001)
                    return;

                var max = 10f;
                var min = 0.25f;
                _zoom = Math.Min(max, Math.Max(min, value));
                _inverseZoom = 1f / _zoom;
                _isDirty = true;
            }
        }

        public float InverseZoom => _inverseZoom;

        private float _moveTimer;
        private Vector2 _position;

        public Vector2 Position
        {
            get => _position;
            private set
            {
                if (Position == value)
                {
                    return;
                }

                _position = value;
                _isDirty = true;
            }
        }

        private bool _mustUpdateVisibleArea;
        private Matrix _transform;
        private Matrix _inverseTransform;

        public Matrix Transform
        {
            get => _transform;
            private set
            {
                _transform = value;
                _inverseTransform = Matrix.Invert(_transform);
                _mustUpdateVisibleArea = true;
            }
        }

        public Matrix UITransform =>
            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
            Matrix.CreateTranslation(new Vector3(0, 0, 0));
            //(_canvasSize.TrueMaxWidth / 2) - (_canvasSize.TrueCurrentWidth / 2),
            //(_canvasSize.TrueMaxHeight / 2) - (_canvasSize.TrueCurrentHeight / 2), 0));

        public Matrix OriginTransform => Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));

        private Rectangle _visibleArea;

        public Rectangle VisibleArea
        {
            get
            {
                if (_mustUpdateVisibleArea)
                {
                    if (Transform == Matrix.Identity)
                        return Rectangle.Empty;

                    var topLeft = Vector2.Transform(Vector2.Zero, _inverseTransform);
                    var topRight = Vector2.Transform(new Vector2(_virtualResolution.MaxWidth, 0), _inverseTransform);
                    var bottomLeft = Vector2.Transform(new Vector2(0, _virtualResolution.MaxHeight), _inverseTransform);
                    var bottomRight = Vector2.Transform(
                        new Vector2(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight),
                        _inverseTransform);

                    var min = new Vector2(
                        MathHelper.Min(topLeft.X,
                            MathHelper.Min(topRight.X, MathHelper.Min(bottomLeft.X, bottomRight.X))),
                        MathHelper.Min(topLeft.Y,
                            MathHelper.Min(topRight.Y, MathHelper.Min(bottomLeft.Y, bottomRight.Y))));
                    var max = new Vector2(
                        MathHelper.Max(topLeft.X,
                            MathHelper.Max(topRight.X, MathHelper.Max(bottomLeft.X, bottomRight.X))),
                        MathHelper.Max(topLeft.Y,
                            MathHelper.Max(topRight.Y, MathHelper.Max(bottomLeft.Y, bottomRight.Y))));

                    _visibleArea = new Rectangle((int) min.X, (int) min.Y, (int) (max.X - min.X),
                        (int) (max.Y - min.Y));
                    _mustUpdateVisibleArea = false;
                }

                return _visibleArea;
            }
        }

        private RectangleF _deadZone;

        public RectangleF DeadZone
        {
            get => new RectangleF(
                _deadZone.X * _virtualResolution.MaxWidth,
                _deadZone.Y * _virtualResolution.MaxHeight,
                _deadZone.Width * _virtualResolution.MaxWidth,
                _deadZone.Height * _virtualResolution.MaxHeight);
            set => _deadZone = value;
        }
        public Rectangle Boundary { get; set; }
        public float Lerp { get; set; }
        public float Lead { get; set; }

        private Vector2 _lastTarget;

        public Camera(VirtualResolution virtualResolution, SpriteBatch spriteBatch, Primitive primitive, GraphicsDeviceManager graphics)
        {
            _virtualResolution = virtualResolution;
            _spriteBatch = spriteBatch;
            _primitive = primitive;
            _graphics = graphics;

            Transform = Matrix.Identity;
        }

        public Vector2 WorldToScreen(Vector2 position)
        {
            return Vector2.Transform(position, _transform);
        }

        public Vector2 ScreenToWorld(Vector2 position)
        {
            return Vector2.Transform(position, _inverseTransform);
        }

        public bool IsVisible(Rectangle bounds)
        {
            return VisibleArea.Contains(bounds)
                   || VisibleArea.Intersects(bounds);
        }

        [System(RunOn.Draw)]
        public void DebugDraw(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin();
            _primitive.DrawBox(DeadZone, Color.YellowGreen, 2);
            _spriteBatch.End();
        }

        [System(RunOn.Update, typeof(CPosition), typeof(CTarget))]
        public void UpdateTransform(GameTime gameTime, GameObjectList gameObjects)
        {
            var target = GetAveragePosition(gameObjects);
            var deadZone = DeadZone;
            var deadZoneOffset = new Vector2((_virtualResolution.MaxWidth / 2) - (_virtualResolution.MaxWidth / 2),
                (_virtualResolution.MaxHeight / 2) - (_virtualResolution.MaxHeight / 2));
            var relativeTarget = WorldToScreen(target);
            var relativeCurrent = WorldToScreen(Position);
            var scroll = Vector2.Zero;

            // figure out how much the camera needs to scroll
            if (relativeTarget.X < relativeCurrent.X + (deadZone.Left + deadZone.Right - relativeCurrent.X))
            {
                var d = relativeTarget.X - deadZone.Left;
                if (d < 0)
                    scroll.X = d;
            }

            if (relativeTarget.X > relativeCurrent.X - (deadZone.Left + deadZone.Right - relativeCurrent.X))
            {
                var d = relativeTarget.X - deadZone.Right;
                if (d > 0)
                    scroll.X = d;
            }

            if (relativeTarget.Y < relativeCurrent.Y + (deadZone.Top + deadZone.Bottom - relativeCurrent.Y))
            {
                var d = relativeTarget.Y - deadZone.Top;
                if (d < 0)
                    scroll.Y = d;
            }

            if (relativeTarget.Y > relativeCurrent.Y - (deadZone.Top + deadZone.Bottom - relativeCurrent.Y))
            {
                var d = relativeTarget.Y - deadZone.Bottom;
                if (d > 0)
                    scroll.Y = d;
            }

            // apply lead
            scroll.X += (target.X - _lastTarget.X) * Lead;
            scroll.Y += (target.Y - _lastTarget.Y) * Lead;

            // scroll towards target
            var position = Position;

            if (_lastTarget != target)
            {
                Position = Vector2.Lerp(Position, Position + scroll, Lerp);
                _moveTimer = 0.05f;
            }
            else
            {
                // add up some time, and if after 2 seconds we haven't moved, start to center the camera.
                _moveTimer -= gameTime.GetElapsedSeconds();

                if (_moveTimer <= 0f)
                    Position = Vector2.Lerp(Position, target, Lerp * 0.5f);
            }

            _lastTarget = target;

            // apply bounds
            if (!Boundary.IsEmpty)
            {
                position.X = Math.Min(Math.Max(Position.X, Boundary.Left + (_virtualResolution.MaxWidth / 2)),
                    Boundary.Right - (_virtualResolution.MaxWidth / 2));
                position.Y = Math.Min(Math.Max(Position.Y, Boundary.Top + (_virtualResolution.MaxHeight / 2)),
                    Boundary.Bottom - (_virtualResolution.MaxHeight / 2));
                Position = position;
            }

            if (_isDirty)
            {
                Transform = Matrix.CreateTranslation(new Vector3( -Position.X, -Position.Y, 0)) *
                            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                            Matrix.CreateTranslation(new Vector3(
                                _virtualResolution.MaxWidth / 2f,
                                _virtualResolution.MaxHeight / 2f,
                                0));
                _isDirty = false;
            }

        }

        private Vector2 GetAveragePosition(GameObjectList gameObjects)
        {
            var total = Vector2.Zero;
            var count = 0;

            foreach (var gameObject in gameObjects)
            {
                total += gameObject.Components.Get<CPosition>().Position;
                if (gameObject.Components.TryGet(out CSize size))
                {
                    total += (size.Size / 2);
                }
                count++;
            }

            if (count == 0)
                return Vector2.Zero;

            total /= count;
            return total;
        }
    }
}
