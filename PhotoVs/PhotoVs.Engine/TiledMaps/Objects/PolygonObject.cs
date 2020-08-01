﻿using System.Collections.Generic;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    public class PolygonObject : BaseObject
    {
        internal PolygonObject(Dictionary<string, string> properties) : base(properties)
        {
        }

        public PolygonObject() : base(new Dictionary<string, string>())
        {
        }

        public Position[] Polygon { get; set; }
    }
}