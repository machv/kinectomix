using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using AtomixData;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Reflection;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace SimpleLevelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            Level level1 = new Level(9, 11);

            for (int y = 0; y < level1.Board.RowsCount; y++)
            {
                for (int x = 0; x < level1.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Type = y == 0 || y == level1.Board.RowsCount - 1 || x == 0 || x == level1.Board.ColumnsCount - 1 ? 
                        TileType.Wall : TileType.Empty;

                    level1.Board[y, x] = tile;
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
                level1.Board[wall.Item1, wall.Item2] = new BoardTile { Type = TileType.Wall };
            }

            //level.Board[2, 2] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            //level.Board[7, 1] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            //level.Board[5, 7] = new BoardTile() { Type = TileType.Oxygen, IsFixed = false };
            level1.Board[1, 4] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            level1.Board[1, 9] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            level1.Board[1, 5] = new BoardTile() { Type = TileType.Oxygen, IsFixed = false };

            // Molecule
            Molecule molecule = new Molecule(1, 3);
            molecule.Name = "Water";
            molecule.Definition[0, 0] = new BoardTile() { Type = TileType.Hydrogen };
            molecule.Definition[0, 1] = new BoardTile() { Type = TileType.Oxygen };
            molecule.Definition[0, 2] = new BoardTile() { Type = TileType.Hydrogen };

            level1.Molecule = molecule;

            // Save definition
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create("../../../Atomix/AtomixContent/Levels/Level1.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, level1, null);
            }

            // Second level
            Level level2 = new Level(9, 11);

            for (int y = 0; y < level2.Board.RowsCount; y++)
            {
                for (int x = 0; x < level2.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Type = y == 0 || y == level2.Board.RowsCount - 1 || x == 0 || x == level2.Board.ColumnsCount - 1 ?
                        TileType.Wall : TileType.Empty;

                    level2.Board[y, x] = tile;
                }
            }

            level2.Board[2, 2] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            level2.Board[7, 1] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            level2.Board[5, 7] = new BoardTile() { Type = TileType.Oxygen, IsFixed = false };

            level2.Molecule = molecule;

            using (XmlWriter writer = XmlWriter.Create("../../../Atomix/AtomixContent/Levels/Level2.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, level2, null);
            }

            // Level 4
            Level level4 = new Level(5, 5);

            for (int y = 0; y < level4.Board.RowsCount; y++)
            {
                for (int x = 0; x < level4.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Type = y == 0 || y == level4.Board.RowsCount - 1 || x == 0 || x == level4.Board.ColumnsCount - 1 ?
                        TileType.Wall : TileType.Empty;

                    level4.Board[y, x] = tile;
                }
            }

            level4.Board[2, 2] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            level4.Board[4, 2] = new BoardTile() { Type = TileType.Hydrogen, IsFixed = false };
            level4.Board[2, 3] = new BoardTile() { Type = TileType.Oxygen, IsFixed = false };

            level4.Molecule = molecule;

            // http://stackoverflow.com/questions/8856528/serialize-texture2d-programatically-in-xna
            Type compilerType = typeof(ContentCompiler);
            ContentCompiler cc = compilerType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(null) as ContentCompiler;
            var compileMethod = compilerType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Instance);
            string fullPath = "../../../Atomix/AtomixContent/Levels/Level4.xnb"; //Path.Combine(@"d:\", "Level.xnb");
            using (FileStream fs = File.Create(fullPath))
            {
                compileMethod.Invoke(cc, new object[]{
      fs, level4, TargetPlatform.Windows, GraphicsProfile.Reach, false/*true*/, fullPath, fullPath
      });
            }

            //IntermediateSerializer.Deserialize

        }
    }
}
