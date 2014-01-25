using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AtomixData
{
    public class BoardTileCollection
    {
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
            Rows = new BoardRow[rows];
            for (int x = 0; x < rows; x++)
                Rows[x] = new BoardRow(columns);
        }
    }
}
