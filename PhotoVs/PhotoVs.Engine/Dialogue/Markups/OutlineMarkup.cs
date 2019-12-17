using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Dialogue.Markups
{
    public class OutlineMarkup : IMarkup
    {
        public Color Color;

        public OutlineMarkup(Color color)
        {
            Color = color;
        }

        public OutlineMarkup(int red, int green, int blue)
        {
            Color = new Color(red, green, blue);
        }
    }
}
