using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    public class Molecule
    {
        public string Name;

        public BoardTileCollection Definition;

        public Molecule() { }

        public Molecule(int rows, int columns)
        {
            Definition = new BoardTileCollection(rows, columns);
        }
    }
}
