using Microsoft.Xna.Framework;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class AnimationFrame
    {
        public float Duration;
        public Rectangle Source;

        public AnimationFrame(Rectangle source, float duration)
        {
            Source = source;
            Duration = duration;
        }
    }
}