using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Dialogue.Markups;

namespace PhotoVs.Engine.Dialogue
{
    public class DialogueMarkup
    {
        private readonly SpriteFont _font;
        private readonly SpriteFont _outlineFont;
        private readonly Dictionary<int, List<IMarkup>> _markupIndex;
        private readonly float _maxCharTime;

        private readonly int _maxLines;
        private readonly Vector2 _origin;

        private readonly Random _rng;
        private readonly string _text;
        private int _breakIndex;
        private float _charTime;
        private int _currentIndex;
        private int _remainingLines;

        public bool FastForward { get; set; }
        public bool IsFinished { get; set; }
        public bool IsPaused { get; set; }

        public DialogueMarkup(SpriteFont font, SpriteFont outlineFont, Vector2 origin, string text, int lines, int width)
        {
            _font = font;
            _outlineFont = outlineFont;
            _origin = origin;
            _maxLines = lines;
            _remainingLines = lines;
            _breakIndex = 0;
            _currentIndex = 0;
            IsPaused = false;
            IsFinished = false;
            _maxCharTime = 1 / 18f;
            _charTime = _maxCharTime;
            FastForward = false;

            (_text, _markupIndex) = ParseMarkup(text);
            _markupIndex = ParseNewLines(_markupIndex, _text, font, width);
            _rng = new Random();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPaused && !IsFinished)
            {
                _charTime -= (float) gameTime.ElapsedGameTime.TotalSeconds * (FastForward ? 25 : 1);

                if (_charTime <= 0)
                {
                    _charTime = _maxCharTime;
                    _currentIndex++;

                    // todo: play a sound effect

                    if (_markupIndex.TryGetValue(_currentIndex, out var activeMarkup))
                    {
                        if (activeMarkup.OfType<NewLineMarkup>().Any())
                        {
                            _remainingLines--;
                            if (_remainingLines <= 0)
                                IsPaused = true;
                        }

                        if (activeMarkup.OfType<EndOfParagraphMarkup>().Any())
                        {
                            _remainingLines = 0;
                            IsPaused = true;
                        }

                        if (activeMarkup.OfType<WaitMarkup>().Any())
                        {
                            var wait = (activeMarkup.First(m => m is WaitMarkup) as WaitMarkup).WaitTime;
                            _charTime = wait;
                        }
                    }

                    if (_currentIndex >= _text.Length)
                        IsFinished = true;
                }
            }

            if (FastForward && IsPaused)
                Next();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_text == string.Empty) return;

            var activeColor = Color.White;
            var outlineColor = Color.Transparent;

            var position = Vector2.Zero;
            var wave = Vector2.Zero;
            var shake = Vector2.Zero;
            for (var i = _breakIndex; i < _currentIndex; i++)
            {
                var c = _text[i];
                var r = _font.GetGlyphs()[c];

                if (_markupIndex.TryGetValue(i, out var activeMarkup))
                {
                    if (activeMarkup.OfType<NewLineMarkup>().Any())
                    {
                        if (i == _breakIndex)
                        {
                            position.Y = 0;

                            if (c == ' ')
                                continue;
                        }
                        else
                        {
                            //if (!(c == ' ' || i == _breakIndex))
                            //{1
                            position.X = 0;
                            position.Y += _font.LineSpacing;
                            //}

                            if (c == ' ')
                                continue;
                        }
                    }

                    if (activeMarkup.OfType<EndOfParagraphMarkup>().Any())
                        if (c == ' ')
                            continue;
                }

                if (activeMarkup != null)
                    foreach (var effect in activeMarkup)
                    {
                        if (effect is ColorMarkup color)
                            activeColor = color.Color;

                        if (effect is WaveMarkup)
                            wave = new Vector2(0,
                                (float) Math.Sin(10 * gameTime.TotalGameTime.TotalSeconds + i * 0.6) * 3 - 1.5f);

                        if (effect is ShakeMarkup)
                            // todo: limit to time
                            shake = new Vector2(_rng.Next(-1, 1), _rng.Next(-1, 1));

                        if (effect is OutlineMarkup outline)
                            outlineColor = outline.Color;
                    }

                // todo: set active colour or position changing
                if (outlineColor != Color.Transparent)
                {
                    spriteBatch.DrawString(_outlineFont, "" + c, _origin + position + wave + shake, outlineColor);
                }

                spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake, activeColor);
                position.X += r.WidthIncludingBearings;

                // reset stuff
                activeColor = Color.White;
                outlineColor = Color.Transparent;
                wave = Vector2.Zero;
                shake = Vector2.Zero;
            }
        }

        public void Next()
        {
            _remainingLines = _maxLines;
            _breakIndex = _currentIndex;
            IsPaused = false;
        }

        private static (string, Dictionary<int, List<IMarkup>>) ParseMarkup(string text)
        {
            var markupIndex = new Dictionary<int, List<IMarkup>>();
            var output = "";
            var index = 0;

            var p = ' '; // previous character

            var isBuffering = false;
            var buffer = "";

            var activeMarkups = new List<IMarkup>();

            foreach (var t in text)
            {
                if (t == '\n')
                {
                    AddMarkup(markupIndex, index, new NewLineMarkup());
                }
                else if (t == '[' && p != '\\')
                {
                    buffer = "";
                    isBuffering = true;
                }
                else if (t == ']' && isBuffering)
                {
                    isBuffering = false;
                    var args = buffer.Split(' ');
                    var markup = args[0].ToLowerInvariant();

                    switch (markup)
                    {
                        case "nl":
                            activeMarkups.Add(new NewLineMarkup());
                            break;

                        case "eop":
                            activeMarkups.Add(new EndOfParagraphMarkup());
                            output += " ";
                            index++;
                            break;

                        case "color":
                            if (args.Length > 1)
                            {
                                var prop = typeof(Color).GetProperty(args[1]);
                                if (prop == null)
                                    activeMarkups.Add(new ColorMarkup(default));
                                else
                                    activeMarkups.Add(new ColorMarkup((Color) prop.GetValue(null, null)));
                            }
                            else
                            {
                                activeMarkups.Add(new ColorMarkup(int.Parse(args[1]), int.Parse(args[2]),
                                    int.Parse(args[3])));
                            }

                            break;

                        case "/color":
                            activeMarkups.RemoveAt(activeMarkups.FindLastIndex(m => m is ColorMarkup));
                            break;

                        case "wave":
                            activeMarkups.Add(new WaveMarkup());
                            break;

                        case "/wave":
                            activeMarkups.RemoveAt(activeMarkups.FindLastIndex(m => m is WaveMarkup));
                            break;

                        case "shake":
                            activeMarkups.Add(new ShakeMarkup());
                            break;

                        case "/shake":
                            activeMarkups.RemoveAt(activeMarkups.FindLastIndex(m => m is ShakeMarkup));
                            break;

                        case "wait":
                            var wait = float.Parse(args[1]);
                            activeMarkups.Add(new WaitMarkup(wait));
                            break;

                        case ".":
                            activeMarkups.Add(new WaitMarkup());
                            break;

                        case "outline":
                            if (args.Length > 1)
                            {
                                var prop = typeof(Color).GetProperty(args[1]);
                                if (prop == null)
                                    activeMarkups.Add(new OutlineMarkup(default));
                                else
                                    activeMarkups.Add(new OutlineMarkup((Color) prop.GetValue(null, null)));
                            }
                            else
                            {
                                activeMarkups.Add(new OutlineMarkup(int.Parse(args[1]), int.Parse(args[2]),
                                    int.Parse(args[3])));
                            }

                            break;

                        case "/outline":
                            activeMarkups.RemoveAt(activeMarkups.FindLastIndex(m => m is OutlineMarkup));
                            break;
                    }
                }
                else
                {
                    if (isBuffering)
                    {
                        buffer += t;
                    }
                    else
                    {
                        if (activeMarkups.Count > 0)
                        {
                            AddMarkups(markupIndex, index, activeMarkups);

                            activeMarkups.RemoveAll(m => m is EndOfParagraphMarkup
                                                         || m is WaitMarkup
                                                         || m is NewLineMarkup);
                        }

                        output += t;
                        index++;
                    }
                }

                p = t;
                //index++;
            }

            return (output, markupIndex);
        }

        private static Dictionary<int, List<IMarkup>> ParseNewLines(Dictionary<int, List<IMarkup>> markupIndex,
            string text, SpriteFont font, int width)
        {
            text += " ";
            var x = 0f;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (markupIndex.TryGetValue(i, out var activeMarkup))
                    if (activeMarkup.OfType<NewLineMarkup>().Any() || activeMarkup.OfType<EndOfParagraphMarkup>().Any())
                        x = 0;

                if (i >= 1 && text[i - 1] == ' ')
                {
                    // check word ahead, make sure it doesn't wrap over
                    var index = text.IndexOf(' ', i + 1);
                    if (index > -1)
                    {
                        var word = text.Substring(i, index - i);
                        var wordWidth = font.MeasureString(word).X;
                        if (x + wordWidth >= width)
                        {
                            x = 0;
                            AddMarkup(markupIndex, i - 1, new NewLineMarkup());
                        }
                    }
                }

                var t = font.GetGlyphs()[c];
                x += t.WidthIncludingBearings;
            }

            return markupIndex;
        }

        private static Dictionary<int, List<IMarkup>> AddMarkup(Dictionary<int, List<IMarkup>> dict, int index,
            IMarkup markup)
        {
            if (!dict.ContainsKey(index))
                dict[index] = new List<IMarkup>();
            dict[index].Add(markup);
            return dict;
        }

        private static Dictionary<int, List<IMarkup>> AddMarkups(Dictionary<int, List<IMarkup>> dict, int index,
            List<IMarkup> markups)
        {
            if (!dict.ContainsKey(index))
                dict[index] = new List<IMarkup>();
            dict[index].AddRange(markups);
            return dict;
        }
    }
}