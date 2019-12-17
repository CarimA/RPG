using System.Collections.Generic;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    public class RectangleObject : BaseObject
    {
        internal RectangleObject(Dictionary<string, string> properties) : base(properties)
        {
        }

        public RectangleObject() : base(new Dictionary<string, string>())
        {
        }
    }
}