using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Logic.PlayerData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace PhotoVs.Logic.Text
{
    public class TextDatabase
    {
        private readonly IAssetLoader _assetLoader;

        private readonly Config _config;
        private readonly Dictionary<Languages, Language> _languages;
        private readonly Player _player;

        public TextDatabase(Services services)
        {
            _config = services.Get<Config>();
            _player = services.Get<Player>();
            _assetLoader = services.Get<IAssetLoader>();

            var deserializer = new Deserializer();
            var sr = new StringReader(_assetLoader.Get<string>("text.yml"));
            var data = deserializer.Deserialize<Dictionary<string, Dictionary<Languages, string>>>(sr);

            _languages = new Dictionary<Languages, Language>();
            foreach (Languages language in Enum.GetValues(typeof(Languages)))
            {
                if (!data["LanguageFont"].ContainsKey(language))
                {
                    continue;
                }

                if (!_languages.ContainsKey(language))
                    _languages.Add(language,
                        new Language(data["Language"][language],
                            _assetLoader.Get<SpriteFont>($"ui/fonts/{data["LanguageFont"][language]}")));

                foreach (var kvp in data.Where(kvp => data[kvp.Key].ContainsKey(language)))
                    _languages[language].Text.Add(kvp.Key, kvp.Value[language]);
            }

            sr.Dispose();
        }

        public SpriteFont GetFont()
        {
            return _languages[_config.Language].Font;
        }

        public string GetText(string id)
        {
            var language = _config.Language;
            if (_languages[language].Text.TryGetValue(id, out var value))
            {
                // parse any embedded language tags
                value = Regex.Replace(value, "\\{= (.+?)\\}", MatchTextMarkup);
                value = Regex.Replace(value, "\\{\\+ (.+?)\\}", MatchFlagMarkup);
            }
            else
            {
                value = $"[{id}:{language.ToString()} NOT SET]";
            }

            return value;
        }

        private string MatchTextMarkup(Match match)
        {
            var id = match.Groups[1].Value;
            return GetText(id);
        }

        private string MatchFlagMarkup(Match match)
        {
            var flag = match.Groups[1].Value;
            return _player.PlayerData.GetVariable(flag).ToString();
        }
    }
}