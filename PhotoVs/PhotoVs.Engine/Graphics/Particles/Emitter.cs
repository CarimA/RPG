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
    public interface IEmitter
    {
        Rectangle Boundaries { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }

    public class Emitter<T> : IEmitter where T : IParticle, new()
    {
        private int _total;
        private Random _random;

        private Texture2D _texture;
        public Rectangle Boundaries { get; }
        private IParticle[] _particles;

        private float _throttleRender;

        public Emitter(int total, Texture2D texture, Rectangle emitterBounds)
        {
            _total = total;
            _particles = new IParticle[total];
            _texture = texture;
            Boundaries = emitterBounds;
            _random = new Random(emitterBounds.GetHashCode());

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
            /*_throttleRender -= gameTime.GetElapsedSeconds();
            if (_throttleRender > 0f)
                return;

            _throttleRender = 1f / 14f;
            var time = TimeSpan.FromSeconds(_throttleRender);*/
            var dTime = gameTime; //new GameTime(TimeSpan.Zero, time);

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
