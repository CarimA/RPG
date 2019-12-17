﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PhotoVs.Engine.TiledMaps.Layers
{
    [XmlRoot("imagelayer")]
    public class ImageLayer : BaseLayer, IXmlSerializable
    {
        [JsonProperty("image")] public string Image { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsStartElement("imagelayer"))
                throw new XmlException();

            LayerType = LayerType.imagelayer;
            Name = reader["name"] ?? throw new KeyNotFoundException("name");
            Visible = reader["visible"].ParseBool() ?? true;
            Opacity = reader["opacity"].ParseDouble() ?? 1.0;

            if (reader.ReadToDescendant("image"))
            {
                Image = reader["source"] ?? throw new KeyNotFoundException("source");
                Width = reader["width"].ParseInt32() ?? -1;
                Height = reader["height"].ParseInt32() ?? -1;
                reader.Read();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}