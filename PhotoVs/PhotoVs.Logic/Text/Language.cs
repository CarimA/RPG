using PhotoVs.Engine.Graphics.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.Text
{
    public class Language
    {
        public string LocalisedName { get; private set; }
        public BitmapFont Font { get; private set; }
        public Dictionary<string, string> Text { get; private set; }

        public Language(string localisedName, BitmapFont font)
        {
            LocalisedName = localisedName;
            Font = font;
            Text = new Dictionary<string, string>();
        }
    }
}
