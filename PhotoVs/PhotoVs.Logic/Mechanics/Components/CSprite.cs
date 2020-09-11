using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class CSprite
    {
        public Texture2D Texture;
        public Vector2 Origin;

        public CSprite(Texture2D texture, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
        }
    }
}
