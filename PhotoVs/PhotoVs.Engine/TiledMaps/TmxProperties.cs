﻿using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace PhotoVs.Engine.TiledMaps
{
    internal static class TmxProperties
    {
        public static void ReadObjectGroup(this XmlReader reader, Dictionary<string, string> properties)
        {
            if (!reader.IsStartElement("objectgroup"))
                throw new XmlException(reader.Name);

            var parent = XNode.ReadFrom(reader) as XElement;
            foreach (var e in parent.Elements())
                properties[parent.Attribute("id").Value] = 
                    e.IsEmpty 
                        ? $"{e.Attribute("name").Value}|{e.Attribute("type").Value}|" +
                          $"{e.Attribute("x").Value}|{e.Attribute("y").Value}|" +
                          $"{e.Attribute("width").Value}|{e.Attribute("height").Value}"
                        : e.Value;
        }

        public static void ReadProperties(this XmlReader reader, Dictionary<string, string> properties)
        {
            if (!reader.IsStartElement("properties"))
                throw new XmlException(reader.Name);

            var parent = XNode.ReadFrom(reader) as XElement;
            foreach (var e in parent.Elements())
                properties[e.Attribute("name").Value] = e.IsEmpty ? e.Attribute("value").Value : e.Value;
        }

        public static void WriteProperties(this XmlWriter writer, Dictionary<string, string> properties)
        {
            if (properties?.Count > 0)
            {
                writer.WriteStartElement("properties");
                foreach (var p in properties)
                {
                    writer.WriteStartElement("property");
                    writer.WriteAttribute("name", p.Key);
                    if (p.Value.Contains("\n"))
                        writer.WriteString(p.Value);
                    else
                        //TODO: Write type
                        writer.WriteAttribute("value", p.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}