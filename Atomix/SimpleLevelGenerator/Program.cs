using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using AtomixData;

namespace SimpleLevelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            Level level = new Level(9, 11);

            //for (int y = 0; y < level.Rows; y++)
            //{
            //    BoardRow row = new BoardRow(level.Columns);
            //    for (int x = 0; x < level.Columns; x++)
            //    {
            //        BoardTile tile = new BoardTile();
            //        tile.Type = y == 0 ? TileType.Wall : TileType.Empty;

            //        row.Columns[x] = tile;
            //    }

            //    level.Board[y] = row;
            //}

            for (int y = 0; y < level.Board.RowsCount; y++)
            {
                for (int x = 0; x < level.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Type = y == 0 || y == level.Board.RowsCount - 1 || x == 0 || x == level.Board.ColumnsCount - 1 ? 
                        TileType.Wall : TileType.Empty;

                    level.Board[y, x] = tile;
                }
            }

            List<Tuple<int, int>> walls = new List<Tuple<int,int>>();
            walls.Add(Tuple.Create<int, int>(1, 3));
            walls.Add(Tuple.Create<int, int>(2, 3));
            walls.Add(Tuple.Create<int, int>(3, 3));
            walls.Add(Tuple.Create<int, int>(3, 2));
            walls.Add(Tuple.Create<int, int>(4, 2));
            walls.Add(Tuple.Create<int, int>(4, 5));
            walls.Add(Tuple.Create<int, int>(4, 7));
            walls.Add(Tuple.Create<int, int>(4, 8));
            walls.Add(Tuple.Create<int, int>(4, 9));

            walls.Add(Tuple.Create<int, int>(6, 1));
            walls.Add(Tuple.Create<int, int>(6, 2));
            walls.Add(Tuple.Create<int, int>(6, 4));
            walls.Add(Tuple.Create<int, int>(5, 5));

            walls.Add(Tuple.Create<int, int>(7, 6));

            foreach(var wall in walls)
            {
                level.Board[wall.Item1, wall.Item2] = new BoardTile { Type = TileType.Wall };
            }

            level.Board[2, 2] = new BoardTile() { Type = TileType.Hydrogen };
            level.Board[7, 1] = new BoardTile() { Type = TileType.Hydrogen };
            level.Board[5, 7] = new BoardTile() { Type = TileType.Oxygen };

            // Molecule
            Molecule molecule = new Molecule(1, 3);
            molecule.Name = "Water";
            molecule.Definition[0, 0] = new BoardTile() { Type = TileType.Hydrogen };
            molecule.Definition[0, 1] = new BoardTile() { Type = TileType.Oxygen };
            molecule.Definition[0, 2] = new BoardTile() { Type = TileType.Hydrogen };

            level.Molecule = molecule;

            // Save definition
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create("Level1.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, level, null);
            }

        }
    }
}
