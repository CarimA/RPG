﻿using System;

namespace PhotoVs.TiledMaps
{
    public struct Frame
    {
        public int TileId { get; set; }
        public int Duration_ms { get; set; }

        public TimeSpan Duration => TimeSpan.FromMilliseconds(Duration_ms);
    }
}