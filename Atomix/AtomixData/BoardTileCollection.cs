using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomixData
{
    [Serializable]
    public class BoardCollection<T> : ICollection<T>
    {
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }

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

        public BoardCollection() { }
        public BoardCollection(int rows, int columns)
        {
            RowsCount = rows;
            ColumnsCount = columns;

            _tiles = new T[rows * columns];
        }

        public void Clear()
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
        public void Add(T item)
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
            foreach (T tile in _tiles)
                yield return tile;
        }
    }
}
