using System.Collections.Generic;

namespace PhotoVs.EditorSuite.GameData
{
    public class Actor
    {
        public Actor()
        {
            Portrait = new Dictionary<Emotion, Portrait>();
        }

        public string NameId { get; set; }
        public Sprite Sprite { get; set; }
        public Dictionary<Emotion, Portrait> Portrait { get; set; } // dictionary used for different emotions/states/etc
        public string Notes { get; set; }
    }
}