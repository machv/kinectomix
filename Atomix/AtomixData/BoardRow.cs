using Microsoft.Xna.Framework;
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
        Carbon,
        Oxygen,
        Hydrogen,
        Up,
        Down,
        Left,
        Right,
    }

    [Flags]
    public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
    }

    public class BoardTile
    {
        public TileType Type { get; set; }
        public bool IsFixed { get; set; }
        public bool IsSelected { get; set; }
        public Direction Movements;
        public Vector2 RenderPosition;

        public BoardTile()
        {
            IsFixed = true;
        }
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
