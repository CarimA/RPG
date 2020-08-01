using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.PlayerData;
using YamlDotNet.Serialization;

namespace PhotoVs.Logic
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Config
    {
        [JsonIgnore] private IAssetLoader _assetloader;

        // General
        public Languages Language { get; set; }

        // Graphics
        public bool Fullscreen { get; set; }

        // Audio
        public bool BgmEnabled { get; set; }
        public int BgmVolume { get; set; }
        public bool SfxEnabled { get; set; }
        public int SfxVolume { get; set; }

        // Controls
        public Dictionary<InputActions, List<Buttons>> ControlsGamepad { get; set; }
        public Dictionary<InputActions, List<Keys>> ControlsKeyboard { get; set; }

        // Screenshot Uploads
        public string DiscordWebhookUrl { get; set; }
        public string ImgurClientId { get; set; }

        public int Deadzone { get; set; }

        public static Config Load(IAssetLoader assetLoader)
        {
            try
            {
                var streamProvider = assetLoader.StreamProvider;
                using var stream = streamProvider.Read(DataLocation.Storage, "config.yml");
                using var reader = new StreamReader(stream);

                var text = reader.ReadToEnd();
                var deserializer = new Deserializer();
                var obj = deserializer.Deserialize<Config>(text);

                if (obj == null)
                {
                    var config = New(assetLoader);
                    config.Save();
                    return config;
                }

                obj._assetloader = assetLoader;

                return obj;
            }
            catch (FileNotFoundException)
            {
                var config = New(assetLoader);
                config.Save();
                return config;
            }
        }

        private void Save()
        {
            var streamProvider = _assetloader.StreamProvider;
            var serializer = new Serializer();
            var data = serializer.Serialize(this);
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            streamWriter.Write(data);
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            streamProvider.Write(DataLocation.Storage, "config.yml", stream);
        }

        private static Config New(IAssetLoader assetLoader)
        {
            var config = new Config
            {
                _assetloader = assetLoader,
                Language = Languages.EnglishUK,

                Fullscreen = false,

                BgmEnabled = true,
                BgmVolume = 100,
                SfxEnabled = true,
                SfxVolume = 100,

                ControlsGamepad = new Dictionary<InputActions, List<Buttons>>
                {
                    [InputActions.Up] = new List<Buttons> {Buttons.DPadUp, Buttons.LeftThumbstickUp},
                    [InputActions.Down] = new List<Buttons> {Buttons.DPadDown, Buttons.LeftThumbstickDown},
                    [InputActions.Left] = new List<Buttons> {Buttons.DPadLeft, Buttons.LeftThumbstickLeft},
                    [InputActions.Right] = new List<Buttons> {Buttons.DPadRight, Buttons.LeftThumbstickRight},
                    [InputActions.Action] = new List<Buttons> {Buttons.A},
                    [InputActions.Submit] = new List<Buttons> {Buttons.Start},
                    [InputActions.Cancel] = new List<Buttons> {Buttons.B},
                    [InputActions.Run] = new List<Buttons> {Buttons.B},
                    [InputActions.Fullscreen] = new List<Buttons> {Buttons.LeftShoulder},
                    [InputActions.Screenshot] = new List<Buttons> {Buttons.Back}
                },
                ControlsKeyboard = new Dictionary<InputActions, List<Keys>>
                {
                    [InputActions.Up] = new List<Keys> {Keys.Up, Keys.W},
                    [InputActions.Down] = new List<Keys> {Keys.Down, Keys.S},
                    [InputActions.Left] = new List<Keys> {Keys.Left, Keys.A},
                    [InputActions.Right] = new List<Keys> {Keys.Right, Keys.D},
                    [InputActions.Action] = new List<Keys> {Keys.Z, Keys.P},
                    [InputActions.Submit] = new List<Keys> {Keys.Enter},
                    [InputActions.Cancel] = new List<Keys> {Keys.X, Keys.O},
                    [InputActions.Run] = new List<Keys> {Keys.X, Keys.O},
                    [InputActions.Fullscreen] = new List<Keys> {Keys.F1},
                    [InputActions.Screenshot] = new List<Keys> {Keys.F12}
                },

                Deadzone = 20,

                DiscordWebhookUrl = "",
                ImgurClientId = ""
            };

            return config;
        }
    }
}