using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Models.Assets;
using PhotoVs.Models.Text;
using YamlDotNet.Serialization;

namespace PhotoVs.Logic.Text
{
    public class TextDatabase : ITextDatabase
    {
        private readonly Player _player;
        private readonly Dictionary<Languages, Language> _languages;

        public TextDatabase(IAssetLoader assetLoader, Player player)
        {
            var deserializer = new Deserializer();
            var sr = new StringReader(assetLoader.GetAsset<string>("text.yml"));
            var data = deserializer.Deserialize<Dictionary<string, Dictionary<Languages, string>>>(sr);

            _languages = new Dictionary<Languages, Language>();
            foreach (Languages language in Enum.GetValues(typeof(Languages)))
            {
                if (!_languages.ContainsKey(language))
                {
                    _languages.Add(language,
                        new Language(data["Language"][language],
                        assetLoader.GetAsset<SpriteFont>($"fonts/{data["LanguageFont"][language]}")));
                }

                foreach (var kvp in data)
                {
                    if (data[kvp.Key].ContainsKey(language))
                    {
                        _languages[language].Text.Add(kvp.Key, kvp.Value[language]);
                    }
                }
            }

            _player = player;
        }

        public SpriteFont GetFont()
        {
            return _languages[_player.Language].Font;
        }

        public string GetText(string id)
        {
            var language = _player.Language;
            if (_languages[language].Text.TryGetValue(id, out var value))
            {
                // parse any embedded language tags
                value = Regex.Replace(value, "\\[text (.+?)\\]", MatchTextMarkup);
                value = Regex.Replace(value, "\\[flag (.+?)\\]", MatchFlagMarkup);
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
            return _player.Flags[flag].ToString();
        }
    }
}