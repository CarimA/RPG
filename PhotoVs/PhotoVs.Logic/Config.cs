using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using YamlDotNet.Serialization;

namespace PhotoVs.Logic
{
    public class Config
    {
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

        public static Config Load()
        {
            try
            {
                var text = File.ReadAllText(GetFileLocation());
                var deserializer = new Deserializer();
                var obj = deserializer.Deserialize<Config>(text);
                return obj;
            }
            catch (FileNotFoundException _)
            {
                var config = New();
                config.Save();
                return config;
            }
        }

        private void Save()
        {
            var serializer = new Serializer();
            var data = serializer.Serialize(this);
            File.WriteAllText(GetFileLocation(), data);
        }

        private static Config New()
        {
            var config = new Config
            {
                Language = Languages.EnglishUK,

                Fullscreen = false,

                BgmEnabled = true,
                BgmVolume = 100,
                SfxEnabled = true,
                SfxVolume = 100,

                ControlsGamepad = new Dictionary<InputActions, List<Buttons>>
                {
                    [InputActions.Up] = new List<Buttons> {Buttons.DPadUp},
                    [InputActions.Down] = new List<Buttons> {Buttons.DPadDown},
                    [InputActions.Left] = new List<Buttons> {Buttons.DPadLeft},
                    [InputActions.Right] = new List<Buttons> {Buttons.DPadRight},
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
                }
            };

            return config;
        }

        private static string GetFileLocation()
        {
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var result = Path.Combine(myDocs, "PhotoVs", "config.yml");
            return result;
        }
    }
}