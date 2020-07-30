using System.Collections.Generic;
using System.Drawing;

namespace PhotoVs.EditorSuite.GameData
{
    public class Portrait
    {
        public string Texture { get; set; }
        public List<Rectangle> LeftEyeSource { get; set; } // sources hold a list of the positions in the textures for animating purposes
        public List<Rectangle> RightEyeSource { get; set; }
        public List<Rectangle> MouthSource { get; set; }
        public Rectangle LeftEye { get; set; }
        public Rectangle RightEye { get; set; }
        public Rectangle Mouth { get; set; }

        public Portrait()
        {
            LeftEyeSource = new List<Rectangle>();
            RightEyeSource = new List<Rectangle>();
            MouthSource = new List<Rectangle>();
        }
    }
}