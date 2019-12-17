using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Dialogue.Markups
{
    public class ColorMarkup : IMarkup
    {
        public Color Color;

        public ColorMarkup(Color color)
        {
            Color = color;
        }

        public ColorMarkup(int red, int green, int blue)
        {
            Color = new Color(red, green, blue);
        }
    }
}
