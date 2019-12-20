using System.Collections.Generic;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    public class PolyLineObject : BaseObject
    {
        public Position[] Polyline { get; set; }

        internal PolyLineObject(Dictionary<string, string> properties) : base(properties)
        {
        }

        public PolyLineObject() : base(new Dictionary<string, string>())
        {
        }
    }
}