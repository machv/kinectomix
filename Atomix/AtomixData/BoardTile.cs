using Microsoft.Xna.Framework;
using System;

namespace AtomixData
{
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
