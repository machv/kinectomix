using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AtomixData
{
    public class BoardTileCollection : ICollection<BoardTile>
    {
        public int RowsCount;
        public int ColumnsCount;

        public BoardRow[] Rows;

        [XmlIgnore]
        public BoardTile this[int row, int column]
        {
            get { return Rows[row].Columns[column]; }
            set { Rows[row].Columns[column] = value; }
        }

        public BoardTileCollection() { }
        public BoardTileCollection(int rows, int columns)
        {
            RowsCount = rows;
            ColumnsCount = columns;

            Rows = new BoardRow[rows];
            for (int x = 0; x < rows; x++)
                Rows[x] = new BoardRow(columns);
        }

        public void Clear()
        {
            foreach (var row in Rows)
            {
                row.Columns = new BoardTile[ColumnsCount];
            }
        }

        public bool Contains(BoardTile item)
        {
            foreach (var row in Rows)
            {
                if (row.Columns.Contains(item))
                    return true;
            }

            return false;
        }

        public void CopyTo(BoardTile[] array, int arrayIndex)
        {
            throw new NotSupportedException("This Method is not valid for this implementation.");
        }

        public void Add(BoardTile item)
        {
            //throw new NotSupportedException("This Method is not valid for this implementation.");
        }

        public int Count
        {
            get { return RowsCount * ColumnsCount; }
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
            foreach (BoardRow row in Rows)
                foreach (BoardTile tile in row.Columns)
                    yield return tile;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (BoardRow row in Rows)
                foreach (BoardTile tile in row.Columns)
                    yield return tile;
        }
    }

    //public class BoardTileEnumerator : IEnumerator<BoardTile>
    //{
    //    protected BoardTileCollection _collection;
    //    protected int index;
    //    protected BoardTile _current;

    //    // Default constructor
    //    public BoardTileEnumerator() { }

    //    // Paramaterized constructor which takes
    //    // the collection which this enumerator will enumerate
    //    public BoardTileEnumerator(BoardTileCollection collection)
    //    {
    //        _collection = collection;
    //        index = -1;
    //        _current = default(BoardTile);
    //    }

    //    public BoardTile Current
    //    {
    //        get { return _current; }
    //    }

    //    public void Dispose()
    //    {
    //        _collection = null;
    //        _current = default(BoardTile);
    //        index = -1;
    //    }

    //    object System.Collections.IEnumerator.Current
    //    {
    //        get { return _current; }
    //    }

    //    public bool MoveNext()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Reset()
    //    {
    //        _current = default(BoardTile);
    //        index = -1;
    //    }
    //}
}
