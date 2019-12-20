﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Dialogue.Markups;
using PhotoVs.Engine.Graphics.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Engine.Dialogue
{
    public class DialogueMarkup
    {
        private readonly BitmapFont _font;
        private readonly string _text;
        private readonly Dictionary<int, List<IMarkup>> _markupIndex;
        private Vector2 _origin;

        private readonly int _maxLines;
        private int _remainingLines;
        private int _breakIndex;
        private int _currentIndex;
        private bool _isPaused;
        private bool _isFinished;
        private readonly float _maxCharTime;
        private float _charTime;

        private bool _fastForwardState;
        private readonly Random _rng;

        public DialogueMarkup(BitmapFont font, Vector2 origin, string text, int lines, int width)
        {
            _font = font;
            _origin = origin;
            _maxLines = lines;
            _remainingLines = lines;
            _breakIndex = 0;
            _currentIndex = 0;
            _isPaused = false;
            _isFinished = false;
            _maxCharTime = 1 / 18f;
            _charTime = _maxCharTime;
            _fastForwardState = false;

            (_text, _markupIndex) = ParseMarkup(text);
            _markupIndex = ParseNewLines(_markupIndex, _text, font, width);
            _rng = new Random();
        }

        public void Update(GameTime gameTime)
        {
            if (!_isPaused && !_isFinished)
            {
                _charTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * (_fastForwardState ? 8 : 1);

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
                            {
                                _isPaused = true;
                            }
                        }

                        if (activeMarkup.OfType<EndOfParagraphMarkup>().Any())
                        {
                            _remainingLines = 0;
                            _isPaused = true;
                        }

                        if (activeMarkup.OfType<WaitMarkup>().Any())
                        {
                            var wait = (activeMarkup.First(m => m is WaitMarkup) as WaitMarkup).WaitTime;
                            _charTime = wait;
                        }
                    }

                    if (_currentIndex >= _text.Length)
                    {
                        _isFinished = true;
                    }

                }
            }

            if (_fastForwardState && _isPaused)
            {
                Next();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var activeColor = Color.White;
            var outlineColor = Color.Transparent;

            var position = Vector2.Zero;
            var wave = Vector2.Zero;
            var shake = Vector2.Zero;
            for (var i = _breakIndex; i < _currentIndex; i++)
            {
                var c = _text[i];
                var r = _font.GetCharacterRegion(c);

                if (r == null)
                {
                    continue;
                }

                if (_markupIndex.TryGetValue(i, out var activeMarkup))
                {
                    if (activeMarkup.OfType<NewLineMarkup>().Any())
                    {
                        if (i == _breakIndex)
                        {
                            position.Y = 0;

                            if (c == ' ')
                            {
                                continue;
                            }
                        }
                        else
                        {

                            //if (!(c == ' ' || i == _breakIndex))
                            //{1
                            position.X = 0;
                            position.Y += _font.LineHeight;
                            //}

                            if (c == ' ')
                            {
                                continue;
                            }
                        }
                    }

                    if (activeMarkup.OfType<EndOfParagraphMarkup>().Any())
                    {
                        if (c == ' ')
                        {
                            continue;
                        }
                    }
                }

                if (activeMarkup != null)
                {
                    foreach (var effect in activeMarkup)
                    {
                        if (effect is ColorMarkup color)
                        {
                            activeColor = color.Color;
                        }

                        if (effect is WaveMarkup)
                        {
                            wave = new Vector2(0, ((float)Math.Sin(10 * gameTime.TotalGameTime.TotalSeconds + (i * 0.6)) * 3) - 1.5f);
                        }

                        if (effect is ShakeMarkup)
                        {
                            // todo: limit to time
                            shake = new Vector2(_rng.Next(-1, 1), _rng.Next(-1, 1));
                        }

                        if (effect is OutlineMarkup outline)
                        {
                            outlineColor = outline.Color;
                        }
                    }
                }

                // todo: set active colour or position changing
                if (outlineColor != Color.Transparent)
                {
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(-1, -1),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(1, 1),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(1, -1),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(-1, 1),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(-1, 0),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(1, 0),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(0, -1),
                        outlineColor);
                    spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake + new Vector2(0, 1),
                        outlineColor);
                }

                spriteBatch.DrawString(_font, "" + c, _origin + position + wave + shake, activeColor);
                position.X += r.Width + r.XOffset - 1;

                // reset stuff
                activeColor = Color.White;
                outlineColor = Color.Black;
                wave = Vector2.Zero;
                shake = Vector2.Zero;
            }
        }

        public void Next()
        {
            _remainingLines = _maxLines;
            _breakIndex = _currentIndex;
            _isPaused = false;
        }

        public bool IsPaused()
        {
            return _isPaused;
        }

        public bool IsFinished()
        {
            return _isFinished;
        }

        public void SetFastForward(bool toggle)
        {
            _fastForwardState = toggle;
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
                                {
                                    activeMarkups.Add(new ColorMarkup(default));
                                }
                                else
                                {
                                    activeMarkups.Add(new ColorMarkup((Color)prop.GetValue(null, null)));
                                }
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
                                {
                                    activeMarkups.Add(new OutlineMarkup(default));
                                }
                                else
                                {
                                    activeMarkups.Add(new OutlineMarkup((Color)prop.GetValue(null, null)));
                                }
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

        private static Dictionary<int, List<IMarkup>> ParseNewLines(Dictionary<int, List<IMarkup>> markupIndex, string text, BitmapFont font, int width)
        {
            text += " ";
            var x = 0f;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (markupIndex.TryGetValue(i, out var activeMarkup))
                {
                    if (activeMarkup.OfType<NewLineMarkup>().Any() || activeMarkup.OfType<EndOfParagraphMarkup>().Any())
                    {
                        x = 0;
                    }
                }

                if (i >= 1 && text[i - 1] == ' ')
                {
                    // check word ahead, make sure it doesn't wrap over
                    var index = text.IndexOf(' ', i + 1);
                    if (index > -1)
                    {
                        var word = text.Substring(i, index - i);
                        var wordWidth = font.MeasureString(word).Width;
                        if (x + wordWidth >= width)
                        {
                            x = 0;
                            AddMarkup(markupIndex, i - 1, new NewLineMarkup());
                        }
                    }
                }

                var t = font.GetCharacterRegion(c);
                if (t != null)
                {
                    x += t.Width + t.XOffset - 1;
                }
            }

            return markupIndex;
        }

        private static Dictionary<int, List<IMarkup>> AddMarkup(Dictionary<int, List<IMarkup>> dict, int index, IMarkup markup)
        {
            if (!dict.ContainsKey(index))
            {
                dict[index] = new List<IMarkup>();
            }
            dict[index].Add(markup);
            return dict;
        }

        private static Dictionary<int, List<IMarkup>> AddMarkups(Dictionary<int, List<IMarkup>> dict, int index, List<IMarkup> markups)
        {
            if (!dict.ContainsKey(index))
            {
                dict[index] = new List<IMarkup>();
            }
            dict[index].AddRange(markups);
            return dict;
        }
    }
}