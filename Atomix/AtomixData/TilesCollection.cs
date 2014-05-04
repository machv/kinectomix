using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AtomixData
{
    [Serializable]
    public class TilesCollection<T> : ICollection<T>, IXmlSerializable where T : new()
    {
        protected int _rowsCount;
        [XmlAttribute]
        public int RowsCount
        {
            get { return _rowsCount; }
            set { _rowsCount = value; }
        }
        protected int _columnsCount;
        [XmlAttribute]
        public int ColumnsCount
        {
            get { return _columnsCount; }
            set { _rowsCount = value; }
        }

        [ContentSerializer]
        protected T[] _tiles;

        protected int GetIndex(int row, int column)
        {
            return row * ColumnsCount + column;
        }

        [ContentSerializerIgnore]
        public T this[int row, int column]
        {
            get { return _tiles[GetIndex(row, column)]; }
            set { _tiles[GetIndex(row, column)] = value; }
        }

        public TilesCollection() { }
        public TilesCollection(int rows, int columns)
        {
            RowsCount = rows;
            ColumnsCount = columns;

            _tiles = new T[rows * columns];
        }

        /// <summary>
        /// Add new empty row to the collection at specified index.
        /// </summary>
        /// <param name="rowIndex"></param>
        public virtual void InsertRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowsCount)
                throw new ArgumentOutOfRangeException("rowIndex");

            int newRowsCount = RowsCount + 1;
            T[] newTiles = new T[ColumnsCount * newRowsCount];

            int index = rowIndex * ColumnsCount;
            Array.Copy(_tiles, 0, newTiles, 0, index);
            Array.Copy(_tiles, index, newTiles, index + ColumnsCount, _tiles.Length - index);

            _tiles = newTiles;
            RowsCount = newRowsCount;
        }

        public void PreprendRow()
        {
            InsertRow(0);
        }

        public void AppendRow()
        {
            InsertRow(RowsCount);
        }

        public virtual void RemoveRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowsCount)
                throw new ArgumentOutOfRangeException("rowIndex");

            int newRowsCount = RowsCount - 1;
            T[] newTiles = new T[ColumnsCount * newRowsCount];

            int index = rowIndex * ColumnsCount;
            Array.Copy(_tiles, 0, newTiles, 0, index);
            Array.Copy(_tiles, index + ColumnsCount, newTiles, index, _tiles.Length - index);

            _tiles = newTiles;
            RowsCount = newRowsCount;
        }

        public virtual void InsertColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex > ColumnsCount)
                throw new ArgumentOutOfRangeException("columnIndex");

            int newColumnsCount = ColumnsCount + 1;
            T[] newTiles = new T[newColumnsCount * RowsCount];

            for (int row = 0; row < RowsCount; row++)
            {
                int index = row * ColumnsCount;
                int newIndex = row * newColumnsCount;

                Array.Copy(_tiles, index, newTiles, newIndex, columnIndex);
                Array.Copy(_tiles, index + columnIndex, newTiles, newIndex + columnIndex + 1, ColumnsCount - columnIndex);
            }

            _tiles = newTiles;
            ColumnsCount = newColumnsCount;
        }

        public virtual void RemoveColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnsCount)
                throw new ArgumentOutOfRangeException("columnIndex");

            int newColumnsCount = ColumnsCount - 1;
            T[] newTiles = new T[newColumnsCount * RowsCount];

            for (int row = 0; row < RowsCount; row++)
            {
                int index = row * ColumnsCount;
                int newIndex = row * newColumnsCount;

                Array.Copy(_tiles, index, newTiles, newIndex, columnIndex);
                Array.Copy(_tiles, index + columnIndex + 1, newTiles, newIndex + columnIndex, newColumnsCount - columnIndex);
            }

            _tiles = newTiles;
            ColumnsCount = newColumnsCount;
        }

        public void AppendColumn()
        {
            InsertColumn(ColumnsCount);
        }

        public void PrependColumn()
        {
            InsertColumn(0);
        }

        public virtual void Clear()
        {
            _tiles = new T[RowsCount * ColumnsCount];
        }

        public bool Contains(T item)
        {
            return _tiles.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _tiles.CopyTo(array, arrayIndex);
        }

        int addingIndex = 0;

        public virtual void Add(T item)
        {
            _tiles[addingIndex++] = item;
        }

        public int Count
        {
            get { return _tiles.Length; }
        }

        // viz http://blog.stephencleary.com/2009/11/icollection-isreadonly-and-arrays.html
        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("This Method is not valid for this implementation.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T tile in _tiles)
                yield return tile;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        // inspired by http://social.msdn.microsoft.com/Forums/en-US/0d94c4f8-767a-4d0f-8c95-f4797cd0ab8e/xmlserializer-doesnt-serialize-attribute-on-listt-subclass?forum=asmxandxml
        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        ///http://stackoverflow.com/questions/4038718/how-can-i-use-reflection-to-convert-from-int-to-decimal
        private object Convert(object source, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (source == null)
                    return null;

                destinationType = Nullable.GetUnderlyingType(destinationType);
            }

            return System.Convert.ChangeType(source, destinationType);
        }

        public void ReadXml(XmlReader reader)
        {
            XmlReader inner = reader.ReadSubtree();

            XmlDocument doc = new XmlDocument();
            doc.Load(inner);
            XmlElement docElem = doc.DocumentElement;

            reader.Read(); // reads Endelement of current node

            // Reflect the [XmlAttribute]'s
            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(XmlAttributeAttribute), false);
                if (attrs != null && attrs.Length == 1)
                {
                    string attr = (attrs[0] as XmlAttributeAttribute).AttributeName;
                    string name = !string.IsNullOrEmpty(attr) ? attr : prop.Name;

                    if (docElem.Attributes[name] != null)
                    {
                        object val = Convert(docElem.Attributes[name].Value, prop.PropertyType);
                        prop.GetSetMethod().Invoke(this, new object[] { val });
                    }
                }
            }

            _tiles = new T[RowsCount * ColumnsCount];

            // Deserialize the collection members
            XmlNodeList nodes = docElem.SelectNodes("./*");
            foreach (XmlNode node in nodes)
            {
                // Make sure it isn't a text node or something
                if (node is XmlElement)
                {
                    XmlElement elem = doc.CreateElement(typeof(T).Name);
                    elem.InnerXml = node.InnerXml;
                    foreach (XmlAttribute xmlAttr in (node as XmlElement).Attributes)
                    {
                        XmlAttribute newAttr = doc.CreateAttribute(xmlAttr.Name);
                        newAttr.Value = xmlAttr.Value;
                        elem.Attributes.Append(newAttr);
                    }

                    this.Add(Serializer.Deserialize<T>(elem));
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            // Reflect the [XmlAttribute]'s
            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(XmlAttributeAttribute), false);
                if (attrs != null && attrs.Length == 1)
                {
                    string name = (attrs[0] as XmlAttributeAttribute).AttributeName;
                    if (string.IsNullOrEmpty(name))
                        name = prop.Name;

                    object value = prop.GetGetMethod().Invoke(this, null);
                    if (value != null)
                        writer.WriteAttributeString(name, value.ToString());
                }
            }

            // Serialize the collection members
            foreach (T item in this)
            {
                string itemName = typeof(T).Name;

                XmlElement serializedItem = Serializer.Serialize<T>(item);
                writer.WriteStartElement(itemName);
                foreach (XmlAttribute xmlAttr in serializedItem.Attributes)
                {
                    // We don't want to write the xsd/xsi namespace attributes
                    if (!(xmlAttr.Name.StartsWith("xmlns:xsd") || xmlAttr.Name.StartsWith("xmlns:xsi")))
                        writer.WriteAttributeString(xmlAttr.Name, xmlAttr.Value);
                }
                writer.WriteRaw(serializedItem.InnerXml);
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
