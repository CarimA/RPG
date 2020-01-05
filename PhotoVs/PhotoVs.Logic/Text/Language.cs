using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Logic.Text
{
    public class Language
    {
        public string LocalisedName { get; private set; }
        public SpriteFont Font { get; private set; }
        public Dictionary<string, string> Text { get; private set; }

        public Language(string localisedName, SpriteFont font)
        {
            LocalisedName = localisedName;
            Font = font;
            Text = new Dictionary<string, string>();
        }
    }
}
