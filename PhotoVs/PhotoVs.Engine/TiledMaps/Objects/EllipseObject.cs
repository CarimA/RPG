using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PhotoVs.Engine.TiledMaps.Objects
{
    public class EllipseObject : BaseObject, IXmlSerializable
    {
        [JsonProperty("ellipse")] public bool IsEllipse { get; set; }

        public EllipseObject(Dictionary<string, string> properties) : base(properties)
        {
        }

        public EllipseObject() : base(new Dictionary<string, string>())
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}