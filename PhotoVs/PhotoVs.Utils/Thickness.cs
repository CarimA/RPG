﻿using System;
using System.Linq;

namespace PhotoVs.Utils
{
    public struct Thickness : IEquatable<Thickness>
    {
        public Thickness(int all)
            : this(all, all, all, all)
        {
        }

        public Thickness(int leftRight, int topBottom)
            : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public Thickness(int left, int top, int right, int bottom)
            : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Width => Left + Right;
        public int Height => Top + Bottom;
        public Size2 Size => new Size2(Width, Height);

        public static implicit operator Thickness(int value)
        {
            return new Thickness(value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Thickness other)
                return Equals(other);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Right;
                hashCode = (hashCode * 397) ^ Bottom;
                return hashCode;
            }
        }

        public bool Equals(Thickness other)
        {
            return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
        }

        public static Thickness FromValues(int[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return values.Length switch
                {
                1 => new Thickness(values[0]),
                2 => new Thickness(values[0], values[1]),
                4 => new Thickness(values[0], values[1], values[2], values[3]),
                _ => throw new FormatException("Invalid thickness"),
                };
        }

        public static Thickness Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var values = value
                .Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            return FromValues(values);
        }

        public override string ToString()
        {
            if (Left == Right && Top == Bottom)
                return Left == Top ? $"{Left}" : $"{Left} {Top}";

            return $"{Left}, {Right}, {Top}, {Bottom}";
        }

        public static bool operator ==(Thickness left, Thickness right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Thickness left, Thickness right)
        {
            return !(left == right);
        }
    }
}