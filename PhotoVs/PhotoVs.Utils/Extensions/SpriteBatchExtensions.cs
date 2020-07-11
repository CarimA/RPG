using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhotoVs.Utils.Extensions
{
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom
    }

    public static class SpriteBatchExtensions
    {
        public static void DrawString(this SpriteBatch spriteBatch,
            SpriteFont font,
            string text,
            Vector2 anchor,
            Color color,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
        {
            if (spriteBatch == null)
                throw new ArgumentNullException(nameof(spriteBatch));

            if (font == null)
                throw new ArgumentNullException(nameof(font));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var splits = text.Split('\n');
            var pos = anchor;

            if (verticalAlignment == VerticalAlignment.Center)
            {
                pos.Y -= (int)((splits.Length * font.LineSpacing) / 2f);
            }

            foreach (var line in splits)
            {
                // -5 is a hack to fix some random padding issue
                var width = font.MeasureString(line).X - 5;

                pos.X = horizontalAlignment switch
                {
                    HorizontalAlignment.Left => anchor.X,
                    HorizontalAlignment.Center => (int) (anchor.X - (width / 2)),
                    HorizontalAlignment.Right => anchor.X - width,
                    _ => pos.X
                };


                spriteBatch.DrawString(font, line, pos, color);

                pos.Y = verticalAlignment switch
                {
                    VerticalAlignment.Top => pos.Y + font.LineSpacing,
                    VerticalAlignment.Center => pos.Y + font.LineSpacing,
                    VerticalAlignment.Bottom => pos.Y - font.LineSpacing,
                    _ => pos.Y
                };
            }
        }

        public static void DrawNineSlice(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destination,
            Rectangle source)
        {
            if (spriteBatch == null)
                throw new ArgumentNullException(nameof(spriteBatch));

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

        public static Color ToColor(this object obj)
        {
            var hashcode = (uint)obj.GetHashCode();
            var color = new Color(hashcode)
            {
                A = byte.MaxValue
            };
            return color;
        }
    }
}