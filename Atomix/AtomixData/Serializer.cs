using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Kinectomix.Logic
{
    public static class Serializer
    {
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
