using Newtonsoft.Json;
using PhotoVs.Engine.TiledMaps.Objects;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PhotoVs.Engine.TiledMaps.Layers
{
    [XmlRoot("objectgroup")]
    public class ObjectLayer : BaseLayer, IXmlSerializable
    {
        [JsonRequired]
        [JsonProperty("objects")]
        public BaseObject[] Objects { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsStartElement("objectgroup"))
                throw new XmlException();

            LayerType = LayerType.objectgroup;
            reader.ReadLayerAttributes(this);

            reader.ReadStartElement("objectgroup");
            var objects = new List<BaseObject>();
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case "properties":
                        reader.ReadProperties(Properties);
                        break;
                    case "object":
                        objects.Add(reader.ReadObject());
                        break;
                    default:
                        throw new XmlException(reader.Name);
                }

            Objects = objects.ToArray();
            if (reader.Name == "objectgroup")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}