using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AtomixData
{
    public class BoardTileCollection : ICollection<BoardTile>
    {
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }

        [ContentSerializer]
        protected BoardTile[] _tiles;

        protected int GetIndex(int row, int column)
        {
            return row * RowsCount + column;
        }

        [ContentSerializerIgnore]
        public BoardTile this[int row, int column]
        {
            get { return _tiles[GetIndex(row, column)]; }
            set { _tiles[GetIndex(row, column)] = value; }
        }

        public BoardTileCollection() { }
        public BoardTileCollection(int rows, int columns)
        {
            RowsCount = rows;
            ColumnsCount = columns;

            _tiles = new BoardTile[rows * columns];
        }

        public void Clear()
        {
            _tiles = new BoardTile[RowsCount * ColumnsCount];
        }

        public bool Contains(BoardTile item)
        {
            return _tiles.Contains(item);
        }

        public void CopyTo(BoardTile[] array, int arrayIndex)
        {
            throw new NotSupportedException("This Method is not valid for this implementation.");
        }

        public void Add(BoardTile item) { }

        public int Count
        {
            get { return _tiles.Length; }
        }

        // viz http://blog.stephencleary.com/2009/11/icollection-isreadonly-and-arrays.html
        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(BoardTile item)
        {
            throw new NotSupportedException("This Method is not valid for this implementation.");
        }

        public IEnumerator<BoardTile> GetEnumerator()
        {
            foreach (BoardTile tile in _tiles)
                yield return tile;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (BoardTile tile in _tiles)
                yield return tile;
        }
    }
}
