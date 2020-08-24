using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Logic.PlayerData;
using YamlDotNet.Serialization;

namespace PhotoVs.Logic.Text
{
    public class TextDatabase : ITextDatabase
    {
        private readonly Config _config;
        private readonly Dictionary<Languages, Language> _languages;
        private readonly Player _player;

        public TextDatabase(IAssetLoader assetLoader, GameState gameState)
        {
            _config = gameState.Config;
            _player = gameState.Player;

            var deserializer = new Deserializer();
            var sr = new StringReader(assetLoader.Get<string>("text.yml"));
            var data = deserializer.Deserialize<Dictionary<string, Dictionary<Languages, string>>>(sr);

            _languages = new Dictionary<Languages, Language>();
            foreach (Languages language in Enum.GetValues(typeof(Languages)))
            {
                if (!data["LanguageFont"].ContainsKey(language)) continue;

                if (!_languages.ContainsKey(language))
                    _languages.Add(language,
                        new Language(data["Language"][language],
                            assetLoader.Get<SpriteFont>($"ui/fonts/{data["LanguageFont"][language]}")));

                foreach (var kvp in data.Where(kvp => data[kvp.Key].ContainsKey(language)))
                    _languages[language].Text.Add(kvp.Key, kvp.Value[language]);
            }

            sr.Dispose();
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

        // todo: refactor this out
        public SpriteFont GetFont()
        {
            return _languages[_config.Language].Font;
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