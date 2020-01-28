using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Utils.Extensions
{
    public static class SpriteFontExtensions
    {
        public static string WrapText(this SpriteFont font, string text, float maxLineWidth)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var words = text.Split(' ');
            var sb = new StringBuilder();
            var lineWidth = 0f;
            var spaceWidth = font.MeasureString(" ").X;

            foreach (var word in words)
            {
                var size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}