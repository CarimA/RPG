using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PhotoVs.Logic.Text
{
    public class Language
    {
        public string LocalisedName { get; }
        public SpriteFont Font { get; }
        public Dictionary<string, string> Text { get; }

        public Language(string localisedName, SpriteFont font)
        {
            LocalisedName = localisedName;
            Font = font;
            Text = new Dictionary<string, string>();
        }
    }
}