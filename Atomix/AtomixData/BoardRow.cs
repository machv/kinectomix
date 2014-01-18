using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    public enum TileType
    {
        Empty,
        Wall,
    }

    public class BoardTile
    {
        public TileType Type;
    }

    public class BoardRow
    {
        public BoardTile[] Columns;

        public BoardRow() { }

        public BoardRow(int columns)
        {
            Columns = new BoardTile[columns];
        }
    }
}
