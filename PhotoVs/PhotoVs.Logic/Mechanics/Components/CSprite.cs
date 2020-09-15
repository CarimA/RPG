using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class CSprite
    {
        [JsonIgnore]
        public Texture2D Texture;
        public Vector2 Origin;

        public CSprite(Texture2D texture, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
        }
    }
}
