using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.Graphics.Particles
{
    public class Emitter<T> : IEmitter where T : IParticle, new()
    {
        private readonly Random _random;
        private readonly int _total;

        private readonly Texture2D _texture;
        public Rectangle Boundaries { get; }
        private readonly IParticle[] _particles;

        private float _throttleRender;

        public Emitter(Random random, int total, Texture2D texture, Rectangle emitterBounds)
        {
            _random = random;
            _total = total;
            _particles = new IParticle[total];
            _texture = texture;
            Boundaries = emitterBounds;

            CreateParticles();
        }

        private void CreateParticles()
        {
            for (var i = 0; i < _total; i++)
                CreateParticle(i, true);
        }

        private void CreateParticle(int index, bool advance = false)
        {
            _particles[index] ??= new T();

            _particles[index].Create(_random, Boundaries);

            if (advance)
            {
                var random = new Random();
                var gameTime = new GameTime();
                var time = TimeSpan.FromSeconds((float)random.NextDouble() * _particles[index].Lifetime);
                gameTime.ElapsedGameTime = time;
                gameTime.TotalGameTime = time;
                UpdateParticle(gameTime, index);
            }
        }


        public void Update(GameTime gameTime)
        {
            _throttleRender -= gameTime.GetElapsedSeconds();
            if (_throttleRender > 0f)
                return;

            _throttleRender = 1f / 15f;
            var time = TimeSpan.FromSeconds(_throttleRender);
            var dTime = new GameTime(TimeSpan.Zero, time);

            for (var i = 0; i < _total; i++)
                UpdateParticle(dTime, i);
        }

        private void UpdateParticle(GameTime gameTime, int index)
        {
            if (_particles[index] == null)
                CreateParticle(index);

            _particles[index].Update(gameTime);

            if (_particles[index].Lifetime <= 0f)
            {
                CreateParticle(index);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var particle in _particles)
            {
                spriteBatch.Draw(_texture, particle.Position, particle.Source, particle.Color, particle.Angle,
                    particle.Origin, particle.Scale, SpriteEffects.None, 1f);
            }
        }
    }
}
