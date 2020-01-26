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

        public static void DrawNineSlice(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destination,
            Rectangle source)
        {
            var sliceWidth = source.Width / 3;
            var sliceHeight = source.Height / 3;

            // top left
            spriteBatch.Draw(texture, new Rectangle(destination.Left, destination.Top, sliceWidth, sliceHeight),
                new Rectangle(source.Left, source.Top, sliceWidth, sliceHeight), Color.White);

            // top right
            spriteBatch.Draw(texture,
                new Rectangle(destination.Right - sliceWidth, destination.Top, sliceWidth, sliceHeight),
                new Rectangle(source.Right - sliceWidth, source.Top, sliceWidth, sliceHeight), Color.White);

            // bottom left
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left, destination.Bottom - sliceHeight, sliceWidth, sliceHeight),
                new Rectangle(source.Left, source.Bottom - sliceHeight, sliceWidth, sliceHeight), Color.White);

            // bottom right
            spriteBatch.Draw(texture,
                new Rectangle(destination.Right - sliceWidth, destination.Bottom - sliceHeight, sliceWidth,
                    sliceHeight),
                new Rectangle(source.Right - sliceWidth, source.Bottom - sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // top
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left + sliceWidth, destination.Top, destination.Width - (sliceWidth * 2),
                    sliceHeight), new Rectangle(source.Left + sliceWidth, source.Top, sliceWidth, sliceHeight),
                Color.White);

            // bottom
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left + sliceWidth, destination.Bottom - sliceHeight,
                    destination.Width - (sliceWidth * 2),
                    sliceHeight),
                new Rectangle(source.Left + sliceWidth, source.Bottom - sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // left
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left, destination.Top + sliceHeight, sliceWidth,
                    destination.Height - (sliceHeight * 2)),
                new Rectangle(source.Left, source.Top + sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // right
            spriteBatch.Draw(texture,
                new Rectangle(destination.Right - sliceWidth, destination.Top + sliceHeight, sliceWidth,
                    destination.Height - (sliceHeight * 2)),
                new Rectangle(source.Right - sliceWidth, source.Top + sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // centre
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left + sliceWidth, destination.Top + sliceHeight,
                    destination.Width - (sliceWidth * 2),
                    destination.Height - (sliceHeight * 2)),
                new Rectangle(source.Left + sliceWidth, source.Top + sliceHeight, sliceWidth, sliceHeight),
                Color.White);
        }
    }
}