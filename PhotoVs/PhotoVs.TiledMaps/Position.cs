﻿using Newtonsoft.Json;

namespace PhotoVs.TiledMaps
{
    public struct Position
    {
        [JsonProperty("x")] public readonly int X;
        [JsonProperty("y")] public readonly int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}