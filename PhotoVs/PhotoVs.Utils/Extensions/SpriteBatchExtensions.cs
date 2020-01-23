using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Utils.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void DrawStringCenterTopAligned(this SpriteBatch spriteBatch,
            SpriteFont font,
            string text,
            Vector2 anchor,
            Color color)
        {
            var pos = anchor;
            foreach (var line in text.Split('\n'))
            {
                var width = font.MeasureString(line).X;
                pos.X = (int) (anchor.X - width / 2);
                spriteBatch.DrawString(font, line, pos, color);
                pos.Y += font.LineSpacing;
            }
        }
    }
}