using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    public class GameProperties
    {
        public GameProperties()
        {
            Languages = new Dictionary<string, string>();
            DefaultSoundEffects = new DefaultSoundEffects();
        }

        // the name of the game, duh
        public string Name { get; set; }

        // the key has the name of the language in english (eg. French)
        // the value has the name of the language in its own language (eg. Francais)
        public Dictionary<string, string> Languages { get; set; }

        public DefaultSoundEffects DefaultSoundEffects { get; set; }
    }
}