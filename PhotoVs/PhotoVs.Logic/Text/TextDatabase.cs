using PhotoVs.Logic.PlayerData;
using PhotoVs.Models.Assets;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace PhotoVs.Logic.Text
{
    public class TextDatabase
    {
        private readonly Player _player;
        private readonly Dictionary<string, Dictionary<string, string>> _text;

        public TextDatabase(IAssetLoader assetLoader, Player player)
        {
            var deserializer = new Deserializer();
            var sr = new StringReader(assetLoader.GetAsset<string>("text.yml"));
            _text = deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(sr);

            _player = player;
            _player.SetLanguage("en-uk");

            var test = GetText("intro");
        }

        public string GetText(string id)
        {
            var language = _player.GetLanguage();
            var value = _text[id][language];

            // parse any embedded language tags
            value = Regex.Replace(value, "\\[text (.+?)\\]", MatchTextMarkup);
            value = Regex.Replace(value, "\\[flag (.+?)\\]", MatchFlagMarkup);

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
            return _player.GetFlag(flag).ToString();
        }
    }
}