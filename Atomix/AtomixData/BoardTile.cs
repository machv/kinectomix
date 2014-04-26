using Microsoft.Xna.Framework;
using System;

namespace AtomixData
{
    /// <summary>
    /// Flag indicating in which directions can be tile moved on the board.
    /// </summary>
    [Flags]
    public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
    }

    /// <summary>
    /// Represents one tile on the game's board.
    /// </summary>
    public class BoardTile
    {
        /// <summary>
        /// Type of tile.
        /// </summary>
        /// <returns>TileType enumeration value.</returns>
        public TileType Type { get; set; }
        /// <summary>
        /// Indicates if this tile is fixed (eg. wall) and cannot be moved during game.
        /// </summary>
        /// <returns>True if tile cannot be moved.</returns>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Constructs new instance of BoardTile class where tile is fixed by default.
        /// </summary>
        public BoardTile()
        {
            IsFixed = true;
        }
    }
}
