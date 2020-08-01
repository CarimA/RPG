using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.NewScenes.SceneComponents
{
    public class HorizontalMenu : List<MenuItem>
    {
        private readonly Rectangle _boundary;
        private readonly SpriteFont _font;
        private int _highlightedIndex;
        private readonly SpriteFont _outlineFont;

        public HorizontalMenu(Rectangle boundary, SpriteFont font, SpriteFont outlineFont)
        {
            TextColor = Color.White;
            _highlightedIndex = 0;
            _boundary = boundary;
            _font = font;
            _outlineFont = outlineFont;
        }

        public Color TextColor { get; set; }
        public Color OutlineColor { get; set; }
        public Color HighlightColor { get; set; }
        public Color HighlightOutlineColor { get; set; }

        public MenuItem HighlightedItem => this[_highlightedIndex];

        public Rectangle GetItemPosition(int index)
        {
            var width = _boundary.Width / Count;

            return new Rectangle(
                _boundary.X + width * index,
                _boundary.Y,
                width,
                _boundary.Height);
        }

        public Rectangle GetHighlightedItemPosition()
        {
            return GetItemPosition(_highlightedIndex);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < Count; i++)
            {
                var position = GetItemPosition(i);
                var vecPosition = new Vector2(position.Center.X, position.Center.Y);
                var item = this[i];

                spriteBatch.DrawString(
                    _outlineFont,
                    item.Text,
                    vecPosition,
                    i == _highlightedIndex ? HighlightOutlineColor : OutlineColor,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);
                spriteBatch.DrawString(
                    _font,
                    item.Text,
                    vecPosition,
                    i == _highlightedIndex ? HighlightColor : TextColor,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);
            }
        }

        public void Next()
        {
            _highlightedIndex++;
            if (_highlightedIndex >= Count)
                _highlightedIndex = 0;
        }

        public void Previous()
        {
            _highlightedIndex--;
            if (_highlightedIndex < 0)
                _highlightedIndex = Count - 1;
        }

        public void Click()
        {
            HighlightedItem.OnClick();
        }
    }
}