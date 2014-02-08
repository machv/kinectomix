using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;
using System.ComponentModel;

namespace AtomixData
{
    public class Level
    {
        public BoardTileCollection Board { get; set; }
        public Molecule Molecule { get; set; }

        public Level() { }

        public Level(int rows, int columns)
        {
            Board = new BoardTileCollection(rows, columns);
        }

        public bool CanGoUp(int x, int y)
        {
            if (x > 0 && Board[x - 1, y].Type == TileType.Empty)
                return true;

            return false;
        }

        public bool CanGoDown(int x, int y)
        {
            if (x + 1 < Board.Rows.Length && Board[x + 1, y].Type == TileType.Empty)
                return true;

            return false;
        }

        public bool CanGoLeft(int x, int y)
        {
            if (y > 0 && Board[x, y - 1].Type == TileType.Empty)
                return true;

            return false;
        }

        public bool CanGoRight(int x, int y)
        {
            if (y + 1 < Board.ColumnsCount && Board[x, y + 1].Type == TileType.Empty)
                return true;

            return false;
        }
    }
}
