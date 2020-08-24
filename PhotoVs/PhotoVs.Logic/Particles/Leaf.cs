using System;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Graphics.Particles;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Particles
{
    public class Leaf : IParticle
    {
        public float Lifetime { get; set; }
        public Rectangle Source { get; set; }
        public Vector2 Origin { get; set; }
        public float Angle { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }

        private float _maxLifetime;
        private Vector2 _velocity;
        private float _rotationSpeed;
        private Color _initialColor;
        private const float _startup = 0.35f;
        private const float _winddown = 2f;

        public void Create(Random random, Rectangle emitterBounds)
        {
            //random = new Random(_velocity.GetHashCode() + GetHashCode() + emitterBounds.GetHashCode());

            Lifetime = random.NextFloat(3, 4.5f);
            _maxLifetime = Lifetime;
            Source = new Rectangle(0, 0, 32, 32);
            Origin = new Vector2(16, 16);
            Angle = 0f;
            Position = random.NextVector2(emitterBounds);
            Scale = Vector2.One;

            _velocity = random.NextVector2(new Vector2(-38f, 14f), new Vector2(-21f, 28f));
            _rotationSpeed = random.NextFloat(0.98f, 3.8f);

            Color = random.NextShuffle(
                //new Color(76, 67, 51), // dark brown
                new Color(79, 81, 42), // darkest green
                new Color(124, 138, 27), // dark green
                new Color(154, 169, 25), // green
                new Color(157, 185, 24) // light green
               // new Color(127, 99, 53) // brown
            );
            _initialColor = Color;
        }

        public void Update(GameTime gameTime)
        {
            Lifetime -= gameTime.GetElapsedSeconds();
            Position += _velocity * gameTime.GetElapsedSeconds();
            Angle += _rotationSpeed * gameTime.GetElapsedSeconds();

            if (Lifetime >= (_maxLifetime - _startup))
            {
                var time = _maxLifetime - Lifetime;
                var progress = time.Map(0, _startup, 0, 1);
                Color = _initialColor * progress;
            }
            else if (Lifetime <= _winddown)
            {
                var time = _winddown - Lifetime;
                var progress = time.Map(0, _winddown, 1, 0);
                _velocity = Vector2.Lerp(_velocity, Vector2.Zero, 1f);
                _rotationSpeed = MathHelper.Lerp(_rotationSpeed, 0, 0.2f);
                Color = _initialColor * progress;
            }
            else
            {
                Color = _initialColor;
            }
            //Color = Color.White * (Lifetime / _maxLifetime);
        }
    }
}
