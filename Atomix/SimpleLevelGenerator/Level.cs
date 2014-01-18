using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLevelGenerator
{
    public class BoardTile
    {
        public TileType Type;
    }

    public class BoardRow
    {
        public BoardTile[] Columns;

        public BoardRow(int columns)
        {
            Columns = new BoardTile[columns];
        }
    }

    public class Level
    {
        public BoardRow[] Rows;
        public int RowsCount;
        public int ColumnsCount;

        public Level(int rows, int columns)
        {
            RowsCount = rows;
            ColumnsCount = columns;

            Rows = new BoardRow[RowsCount];
        }
    }
}
