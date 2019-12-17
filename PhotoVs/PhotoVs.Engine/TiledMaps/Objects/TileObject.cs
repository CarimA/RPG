﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    public class TileObject : BaseObject
    {
        internal TileObject(Dictionary<string, string> properties) : base(properties)
        {
        }

        public TileObject() : base(new Dictionary<string, string>())
        {
        }

        [JsonProperty("gid")] public int Gid { get; set; }
    }
}