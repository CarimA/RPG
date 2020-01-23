using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Properties;
using PhotoVs.Models.ECS;

namespace PhotoVs.Logic.Input
{
    public class STakeScreenshot : IUpdateableSystem
    {
        private readonly SpriteFont _font;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Renderer _renderer;
        private readonly SpriteBatch _spriteBatch;

        public STakeScreenshot(GraphicsDevice graphicsDevice, Renderer renderer, SpriteBatch spriteBatch,
            SpriteFont font)
        {
            _graphicsDevice = graphicsDevice;
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _font = font;
        }

        public int Priority { get; set; } = 999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = {typeof(CInput)};

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, IGameObjectCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInput>().Input;
                if (input.ActionPressed(InputActions.Screenshot))
                    TakeScreenshot();
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }

        private void TakeScreenshot()
        {
            var width = _graphicsDevice.PresentationParameters.BackBufferWidth;
            var height = _graphicsDevice.PresentationParameters.BackBufferHeight;

            var sWidth = (int) _renderer.RenderSize.Width * 4;
            var sHeight = (int) _renderer.RenderSize.Height * 4;

            var data = new Color[width * height];
            _graphicsDevice.GetBackBufferData(data);
            using var tex = new Texture2D(_graphicsDevice, width, height);
            tex.SetData(data);

            var rt = new RenderTarget2D(_graphicsDevice, sWidth, sHeight);

            _graphicsDevice.SetRenderTarget(rt);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(tex, new Rectangle(0, 0, sWidth, sHeight), Color.White);

            var text = "PhotoVs - Development Build - discord.gg/ew2X8Sy";
            var size = _font.MeasureString(text);

            var x = sWidth / 2 - size.X / 2;
            var y = sHeight - 20 - size.Y;
            var t = 2;

            _spriteBatch.DrawString(_font, text, new Vector2(x + t, y - t), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x + t, y + t), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x - t, y - t), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x - t, y + t), Color.Black);

            _spriteBatch.DrawString(_font, text, new Vector2(x + t, y), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x - t, y), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x, y - t), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(x, y + t), Color.Black);

            _spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.Yellow);

            _spriteBatch.End();
            _graphicsDevice.SetRenderTarget(null);

            using var stream = File.Create(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"PhotoVs/Screenshots/{DateTime.Now.ToString("yyyyMMdd-HHmmss")}-{Guid.NewGuid().ToString()}.png"));
            rt.SaveAsPng(stream, sWidth, sHeight);

            UploadToDiscord(rt);

            rt.Dispose();
            stream.Dispose();
        }

        private async void UploadToDiscord(RenderTarget2D rt)
        {
            using var ms = new MemoryStream();

            rt.SaveAsPng(ms, 1280, 720);

            using var request = new WebClient();

            var clientId = Resources.ResourceManager.GetString("IMGUR_CLIENT_ID");
            request.Headers.Add("Authorization", "Client-ID " + clientId);
            var values = new NameValueCollection {{"image", Convert.ToBase64String(ms.GetBuffer())}};
            var res = request.UploadValues("https://api.imgur.com/3/upload.xml", values);

            using var response = new MemoryStream(res);

            foreach (var data in XDocument.Load(response).Descendants("data"))
            {
                var val = data.Element("link").Value;

                var embed = "{\"embeds\":[{\"image\":{\"url\":\"" + val + "\"}}]}";
                var desc = Interaction.InputBox("Want to add a description?");

                if (desc != string.Empty)
                    embed = "{\"embeds\":[{\"description\":\"" + desc + "\", \"image\":{\"url\":\"" + val + "\"}}]}";

                Clipboard.SetText(val);
                await new HttpClient().PostAsync(Resources.ResourceManager.GetString("DISCORD_WEBHOOK_URL"),
                    new StringContent(embed, Encoding.UTF8, "application/json"));
            }
        }
    }
}