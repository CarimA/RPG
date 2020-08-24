using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Graphics.Particles
{
    public interface IParticle
    {
        float Lifetime { get; set; }
        Rectangle Source { get; set; }
        Vector2 Origin { get; set; }
        float Angle { get; set; }
        Vector2 Position { get; set; }
        Color Color { get; set; }
        Vector2 Scale { get; set; }

        void Create(Random random, Rectangle emitterBounds);
        void Update(GameTime gameTime);
    }
}
