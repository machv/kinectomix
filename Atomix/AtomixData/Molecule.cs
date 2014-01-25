using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    public class Molecule
    {
        public int Rows;
        public int Columns;

        public string Name;

        public BoardTileCollection Definition;

        public Molecule() { }

        public Molecule(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;

            Definition = new BoardTileCollection(rows, columns);
        }
    }
}
