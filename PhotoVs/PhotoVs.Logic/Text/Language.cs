using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Logic.Text
{
    public class Language
    {
        public Language(string localisedName, SpriteFont font)
        {
            LocalisedName = localisedName;
            Font = font;
            Text = new Dictionary<string, string>();
        }

        public string LocalisedName { get; }
        public SpriteFont Font { get; }
        public Dictionary<string, string> Text { get; }
    }
}