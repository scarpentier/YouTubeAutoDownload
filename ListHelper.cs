using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace YouTubeFavDownload
{
    internal static class ListExtension
    {
        public static void LoadXml<T>(this List<T> list, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var stream = File.OpenWrite(fileName))
            {
                serializer.Serialize(stream, list);
            }
        }

        public static void SaveXml<T>(this List<T> list, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var stream = File.OpenRead(fileName))
            {
                var other = (List<T>)(serializer.Deserialize(stream));
                list.Clear();
                list.AddRange(other);
            }
        }

        public static IList LoadXml(this IList list, string fileName)
        {
            var serializer = new XmlSerializer(typeof (IEnumerable));
            var fs = new FileStream(fileName, FileMode.Open);
            return (IList) serializer.Deserialize(fs);
        }

        public static void SaveXml(this IList list, string fileName)
        {
            var serializer = new XmlSerializer(typeof (IList));
            var fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, list);
            fs.Close();
        }
    }

    [XmlRoot("dictionary")]
    internal class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(string fileName)
        {
            using (var reader = XmlReader.Create(fileName))
            {
                reader.ReadStartElement(); // Root node
                this.ReadXml(reader);
                reader.Close();
            }
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof (TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof (TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey) keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue) valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            if (reader.NodeType != XmlNodeType.None)
                reader.ReadEndElement();
        }

        /// <summary>
        /// Write the dictionnary to an XML file
        /// </summary>
        /// <param name="fileName">Path the file</param>
        /// <param name="root">Title of the root element</param>
        public void WriteXml(string fileName, string root)
        {
            var settings = new XmlWriterSettings
                               {
                                   ConformanceLevel = ConformanceLevel.Auto,
                                   Indent = true,
                                   NewLineChars = Environment.NewLine
                               };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(root);
                this.WriteXml(writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof (TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof (TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}