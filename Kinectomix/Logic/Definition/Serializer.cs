using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Custom XML serializer for serializing <see cref="TilesCollection{T}"/>.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Deserializes the specified node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(XmlElement node) where T : new()
        {
            T customType = new T();
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", String.Empty));
            doc.AppendChild(doc.ImportNode(node, true));

            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            using (StreamReader reader = new StreamReader(stream))
            {
                doc.Save(writer);
                stream.Position = 0;
                customType = (T)serializer.Deserialize(reader);
            }

            return customType;
        }

        /// <summary>
        /// Serializes the specified element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">Element to serialize.</param>
        /// <returns>Serialized XML form of specified instance.</returns>
        public static XmlElement Serialize<T>(T t) where T : new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlElement elem;
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            using (StreamReader reader = new StreamReader(stream))
            {
                serializer.Serialize(writer, t);

                XmlDocument doc = new XmlDocument();
                stream.Position = 0;
                doc.Load(reader);
                elem = doc.DocumentElement;
            }

            return elem;
        }
    }
}
