using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AtomixData
{
    public class BoardTileCollection
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

        public bool CanGoUp(int x, int y)
        {
            if (x > 0 && this[x - 1, y].Type == TileType.Empty)
                return true;

            return false;
        }

        public bool CanGoDown(int x, int y)
        {
            if (x + 1 < Rows.Length && this[x + 1, y].Type == TileType.Empty)
                return true;

            return false;
        }

        public bool CanGoLeft(int x, int y)
        {
            if (y > 0 && this[x, y - 1].Type == TileType.Empty)
                return true;

            return false;
        }

        public bool CanGoRight(int x, int y)
        {
            if (y + 1 < ColumnsCount && this[x, y + 1].Type == TileType.Empty)
                return true;

            return false;
        }
    }
}
