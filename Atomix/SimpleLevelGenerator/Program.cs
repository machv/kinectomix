using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using KinectomixLogic;
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

            Level level1 = new Level();
            level1.Board = new TilesCollection<BoardTile>(9, 11);

            for (int y = 0; y < level1.Board.RowsCount; y++)
            {
                for (int x = 0; x < level1.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Asset = y == 0 || y == level1.Board.RowsCount - 1 || x == 0 || x == level1.Board.ColumnsCount - 1 ?
                        "Wall" : "Empty";

                    level1.Board[y, x] = tile;
                }
            }

            List<Tuple<int, int>> walls = new List<Tuple<int, int>>();
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

            foreach (var wall in walls)
            {
                level1.Board[wall.Item1, wall.Item2] = new BoardTile { Asset = "Wall" };
            }

            //level.Board[2, 2] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            //level.Board[7, 1] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            //level.Board[5, 7] = new BoardTile() { Asset = "Oxygen", IsFixed = false };
            level1.Board[1, 4] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            level1.Board[1, 9] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            level1.Board[1, 5] = new BoardTile() { Asset = "Oxygen", IsFixed = false };

            level1.Name = "Water";

            // Molecule
            TilesCollection<BoardTile> molecule = new TilesCollection<BoardTile>(1, 3);
            molecule[0, 0] = new BoardTile() { Asset = "Hydrogen" };
            molecule[0, 1] = new BoardTile() { Asset = "Oxygen" };
            molecule[0, 2] = new BoardTile() { Asset = "Hydrogen" };

            level1.Molecule = molecule;

            // Save definition
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create("../../../Atomix/AtomixContent/Levels/Level1.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, level1, null);
            }

            // Second level
            Level level2 = new Level();
            level2.Board = new TilesCollection<BoardTile>(9, 11);

            for (int y = 0; y < level2.Board.RowsCount; y++)
            {
                for (int x = 0; x < level2.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Asset = y == 0 || y == level2.Board.RowsCount - 1 || x == 0 || x == level2.Board.ColumnsCount - 1 ?
                        "Wall" : "Empty";

                    level2.Board[y, x] = tile;
                }
            }

            level2.Board[2, 2] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            level2.Board[7, 1] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            level2.Board[5, 7] = new BoardTile() { Asset = "Oxygen", IsFixed = false };

            level2.Molecule = molecule;

            using (XmlWriter writer = XmlWriter.Create("../../../Atomix/AtomixContent/Levels/Level2.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, level2, null);
            }

            // Level 4
            Level level4 = new Level();
            level4.Board = new TilesCollection<BoardTile>(5, 5);

            for (int y = 0; y < level4.Board.RowsCount; y++)
            {
                for (int x = 0; x < level4.Board.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Asset = y == 0 || y == level4.Board.RowsCount - 1 || x == 0 || x == level4.Board.ColumnsCount - 1 ?
                        "Wall" : "Empty";

                    level4.Board[y, x] = tile;
                }
            }

            level4.Board[2, 2] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            level4.Board[4, 2] = new BoardTile() { Asset = "Hydrogen", IsFixed = false };
            level4.Board[2, 3] = new BoardTile() { Asset = "Oxygen", IsFixed = false };

            level4.Molecule = molecule;

            // http://stackoverflow.com/questions/8856528/serialize-texture2d-programatically-in-xna
            Type compilerAsset = typeof(ContentCompiler);
            ContentCompiler cc = compilerAsset.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(null) as ContentCompiler;
            var compileMethod = compilerAsset.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Instance);
            string fullPath = "../../../Atomix/AtomixContent/Levels/Level4.xnb"; //Path.Combine(@"d:\", "Level.xnb");
            using (FileStream fs = File.Create(fullPath))
            {
                compileMethod.Invoke(cc, new object[]{
      fs, level4, TargetPlatform.Windows, GraphicsProfile.Reach, false/*true*/, fullPath, fullPath
      });
            }

            LevelDefinition[] levels = new LevelDefinition[3];

            levels[0] = new LevelDefinition() { FileName = "Level1.xml", Name = "Level 1" };
            levels[1] = new LevelDefinition() { FileName = "Level2.xml", Name = "Level 2" };
            levels[2] = new LevelDefinition() { FileName = "Level4.xnb", Name = "Level 4" };

            using (XmlWriter writer = XmlWriter.Create("../../../Atomix/AtomixContent/Levels.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, levels, null);
            }
        }
    }
}
