using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Provides two dimensional collection.
    /// </summary>
    /// <typeparam name="T">Containing tile inside collection.</typeparam>
    [Serializable]
    public class TilesCollection<T> : ICollection<T>, IXmlSerializable where T : new()
    {
        private int _addingIndex = 0;

        /// <summary>
        /// Internal one-dimensional representation of this collection.
        /// </summary>
        protected T[] _tiles;
        /// <summary>
        /// The count of rows in this collection
        /// </summary>
        protected int _rowCount;
        /// <summary>
        /// The columns count in this collection.
        /// </summary>
        protected int _columnCount;

        /// <summary>
        /// Gets or sets the rows count in this collection.
        /// </summary>
        /// <value>
        /// The rows count.
        /// </value>
        [XmlAttribute]
        public virtual int RowsCount
        {
            get { return _rowCount; }
            set { _rowCount = value; }
        }
        /// <summary>
        /// Gets or sets the columns count in this collection.
        /// </summary>
        /// <value>
        /// The columns count.
        /// </value>
        [XmlAttribute]
        public virtual int ColumnsCount
        {
            get { return _columnCount; }
            set { _columnCount = value; }
        }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="TilesCollection{T}" />.
        /// </summary>
        public int Count
        {
            get { return _tiles.Length; }
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="TilesCollection{T}" /> is read-only. Always returns true.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; } // viz http://blog.stephencleary.com/2009/11/icollection-isreadonly-and-arrays.html
        }
        /// <summary>
        /// Gets or sets the item at specified coordinates in this two-dimensional collection.
        /// </summary>
        /// <value>
        /// The Item at specified coordinates.
        /// </value>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns></returns>
        public virtual T this[int row, int column]
        {
            get { return _tiles[GetIndex(row, column)]; }
            set { _tiles[GetIndex(row, column)] = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TilesCollection{T}"/> class.
        /// </summary>
        public TilesCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TilesCollection{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public TilesCollection(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;

            _tiles = new T[rowCount * columnCount];
        }

        /// <summary>
        /// Add new empty row to the collection at specified index.
        /// </summary>
        /// <param name="rowIndex"></param>
        public virtual void InsertRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex > _rowCount)
                throw new ArgumentOutOfRangeException("rowIndex");

            int newRowsCount = RowsCount + 1;
            T[] newTiles = new T[ColumnsCount * newRowsCount];

            int index = rowIndex * ColumnsCount;
            Array.Copy(_tiles, 0, newTiles, 0, index);
            Array.Copy(_tiles, index, newTiles, index + ColumnsCount, _tiles.Length - index);

            _tiles = newTiles;
            RowsCount = newRowsCount;
        }

        /// <summary>
        /// Inserts new row at the beginning.
        /// </summary>
        public void PreprendRow()
        {
            InsertRow(0);
        }

        /// <summary>
        /// Inserts new row at the end.
        /// </summary>
        public void AppendRow()
        {
            InsertRow(RowsCount);
        }

        /// <summary>
        /// Removes row at specified index.
        /// </summary>
        /// <param name="rowIndex">Index of the row to remove.</param>
        public virtual void RemoveRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowsCount)
                throw new ArgumentOutOfRangeException("rowIndex");

            int newRowsCount = RowsCount - 1;
            T[] newTiles = new T[ColumnsCount * newRowsCount];

            int index = rowIndex * ColumnsCount;
            Array.Copy(_tiles, 0, newTiles, 0, index);
            Array.Copy(_tiles, index + ColumnsCount, newTiles, index, _tiles.Length - index - _columnCount);

            _tiles = newTiles;
            _rowCount = newRowsCount;
        }

        /// <summary>
        /// Inserts new column at specified index.
        /// </summary>
        /// <param name="columnIndex">Index of the column to add.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">columnIndex</exception>
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
            _columnCount = newColumnsCount;
        }

        /// <summary>
        /// Removes the column at specified index.
        /// </summary>
        /// <param name="columnIndex">Index of the column to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">columnIndex</exception>
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
            _columnCount = newColumnsCount;
        }

        /// <summary>
        /// Inserts new column at the end.
        /// </summary>
        public void AppendColumn()
        {
            InsertColumn(ColumnsCount);
        }

        /// <summary>
        /// Inserts new column at the beginning.
        /// </summary>
        public void PrependColumn()
        {
            InsertColumn(0);
        }

        /// <summary>
        /// Removes all items from the <see cref="TilesCollection{T}"/>
        /// </summary>
        public virtual void Clear()
        {
            _tiles = new T[RowsCount * ColumnsCount];
            _addingIndex = 0;
        }

        /// <summary>
        /// Determines whether collection contains the specified item.
        /// </summary>
        /// <param name="item">The item to look up.</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _tiles.Contains(item);
        }

        /// <summary>
        /// Copies all items from this collection to specified one-dimensional array starting at specified index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Starting index in the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _tiles.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Adds the item at the end of the array.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Add(T item)
        {
            _tiles[_addingIndex++] = item;
        }

        /// <summary>
        /// Removes the specified item from the <see cref="TilesCollection{T}"/>.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">This Method is not valid for this implementation.</exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException("This Method is not valid for this implementation.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T tile in _tiles)
                yield return tile;
        }

        /// <summary>
        /// Gets the index into the one-dimensional array that corresponds to the two-dimensional indexes.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns></returns>
        protected int GetIndex(int row, int column)
        {
            return row * _columnCount + column;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        // inspired by http://social.msdn.microsoft.com/Forums/en-US/0d94c4f8-767a-4d0f-8c95-f4797cd0ab8e/xmlserializer-doesnt-serialize-attribute-on-listt-subclass?forum=asmxandxml
        #region IXmlSerializable Members
        /// <summary>
        /// Gets the <see cref="XmlSchema"/>.
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads the XML serialized data and deserializes items.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void ReadXml(XmlReader reader)
        {
            XmlReader inner = reader.ReadSubtree();

            XmlDocument doc = new XmlDocument();
            doc.Load(inner);
            XmlElement docElem = doc.DocumentElement;

            reader.Read(); // reads Endelement of current node

            // Reflect the [XmlAttribute]'s
            PropertyInfo[] props = GetType().GetProperties();
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
                    XmlAttributeCollection attributes = (node as XmlElement).Attributes;
                    if (attributes["xsi:nil"] != null && attributes["xsi:nil"].Value.ToLower() == "true")
                    {
                        Add(default(T));

                        continue;
                    }

                    XmlElement elem = doc.CreateElement(typeof(T).Name);
                    elem.InnerXml = node.InnerXml;
                    foreach (XmlAttribute xmlAttr in (node as XmlElement).Attributes)
                    {
                        XmlAttribute newAttr = doc.CreateAttribute(xmlAttr.Name);
                        newAttr.Value = xmlAttr.Value;
                        elem.Attributes.Append(newAttr);
                    }

                    Add(Serializer.Deserialize<T>(elem));
                }
            }
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
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

        //http://stackoverflow.com/questions/4038718/how-can-i-use-reflection-to-convert-from-int-to-decimal
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
        #endregion
    }
}
