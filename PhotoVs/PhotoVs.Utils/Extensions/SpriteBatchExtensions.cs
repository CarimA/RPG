using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace PhotoVs.Utils.Extensions
{
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
                pos.Y -= (int) (splits.Length * font.LineSpacing / 2f);
            }

            foreach (var line in splits)
            {
                // -5 is a hack to fix some random padding issue
                var width = font.MeasureString(line).X - 5;

                pos.X = horizontalAlignment switch
                {
                    HorizontalAlignment.Left => anchor.X,
                    HorizontalAlignment.Center => (int) (anchor.X - width / 2),
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
                new Rectangle(destination.Left + sliceWidth, destination.Top, destination.Width - sliceWidth * 2,
                    sliceHeight), new Rectangle(source.Left + sliceWidth, source.Top, sliceWidth, sliceHeight),
                Color.White);

            // bottom
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left + sliceWidth, destination.Bottom - sliceHeight,
                    destination.Width - sliceWidth * 2,
                    sliceHeight),
                new Rectangle(source.Left + sliceWidth, source.Bottom - sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // left
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left, destination.Top + sliceHeight, sliceWidth,
                    destination.Height - sliceHeight * 2),
                new Rectangle(source.Left, source.Top + sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // right
            spriteBatch.Draw(texture,
                new Rectangle(destination.Right - sliceWidth, destination.Top + sliceHeight, sliceWidth,
                    destination.Height - sliceHeight * 2),
                new Rectangle(source.Right - sliceWidth, source.Top + sliceHeight, sliceWidth, sliceHeight),
                Color.White);

            // centre
            spriteBatch.Draw(texture,
                new Rectangle(destination.Left + sliceWidth, destination.Top + sliceHeight,
                    destination.Width - sliceWidth * 2,
                    destination.Height - sliceHeight * 2),
                new Rectangle(source.Left + sliceWidth, source.Top + sliceHeight, sliceWidth, sliceHeight),
                Color.White);
        }

        public static Color ToColor(this object obj)
        {
            var hashcode = (uint) obj.GetHashCode();
            var color = new Color(hashcode)
            {
                A = byte.MaxValue
            };
            return color;
        }

        public static Bitmap GetBitmap(this Texture2D texture)
        {
            var textureData = new uint[texture.Width * texture.Height];
            texture.GetData(textureData);

            var bmp = new Bitmap(texture.Width, texture.Height, PixelFormat.Format32bppArgb);

            unsafe
            {
                var origdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.WriteOnly, bmp.PixelFormat);
                var byteData = (uint*)origdata.Scan0;
                for (var i = 0; i < textureData.Length; i++)
                {
                    byteData[i] = (textureData[i] & 0x000000ff) << 16 | (textureData[i] & 0x0000FF00) |
                                  (textureData[i] & 0x00FF0000) >> 16 | (textureData[i] & 0xFF000000);
                }

                bmp.UnlockBits(origdata);
            }

            return bmp;
        }

        public static void SaveAsPng(this Texture2D texture, string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            /*using var stream = File.Create(filename);
            {
                texture.SaveAsPng(stream, texture.Width, texture.Height);
            }*/

            var bmp = texture.GetBitmap();
            using var stream = File.Create(filename);
            bmp.Save(stream, ImageFormat.Png);
        }

        public static string UploadToImgur(this Texture2D texture, string clientId)
        {
            var bmp = texture.GetBitmap();
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            using var request = new WebClient();
            request.Headers.Add("Authorization", "Client-ID " + clientId);
            var values = new NameValueCollection { { "image", Convert.ToBase64String(ms.GetBuffer()) } };
            var res = request.UploadValues("https://api.imgur.com/3/upload.xml", values);

            using var response = new MemoryStream(res);

            var str = Encoding.UTF8.GetString(response.ToArray());

            var startIndex = str.IndexOf("<link>");
            var endIndex = str.IndexOf("</link>");

            if (startIndex == -1 || endIndex == -1)
                return string.Empty;

            startIndex += "<link>".Length;
            var url = str.Substring(startIndex, endIndex - startIndex);

            return url;
        }

        public static async void UploadToDiscord(this Texture2D texture, string imgurClientId, string discordWebhook)
        {
            var imgurUrl = texture.UploadToImgur(imgurClientId);
            var embed = "{\"embeds\":[{\"image\":{\"url\":\"" + imgurUrl + "\"}}]}";
            await new HttpClient().PostAsync(discordWebhook,
                new StringContent(embed, Encoding.UTF8, "application/json"));
        }
    }
}