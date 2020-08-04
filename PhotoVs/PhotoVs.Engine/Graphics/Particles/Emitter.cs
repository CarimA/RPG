using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils.Collections.Pooling;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.Graphics.Particles
{
    public class Emitter<T> where T : IParticle, new()
    {
        private int _total;
        private Random _random;

        private Texture2D _texture;
        private Rectangle _source;
        private IParticle[] _particles;

        public Emitter(int total, Texture2D texture, Rectangle emitterBounds)
        {
            _total = total;
            _particles = new IParticle[total];
            _texture = texture;
            _source = emitterBounds;
            _random = new Random();

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

            _particles[index].Create(_random, _source);

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
            for (var i = 0; i < _total; i++)
                UpdateParticle(gameTime, i);
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
