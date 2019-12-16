﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhotoVs.TiledMaps
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StaggerAxis : byte
    {
        None = 0,
        [EnumMember(Value = "x")] x,
        [EnumMember(Value = "y")] y
    }
}