using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Models.Text
{
    public interface ITextDatabase
    {
        string GetText(string id);
        SpriteFont GetFont();
    }
}