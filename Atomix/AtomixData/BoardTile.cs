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
        /// Image asset representing this tile.
        /// </summary>
        /// <returns>File name of asset.</returns>
        public string Asset { get; set; }
        /// <summary>
        /// Indicates if this tile is fixed (eg. wall) and cannot be moved during game.
        /// </summary>
        /// <returns>True if tile cannot be moved.</returns>
        public bool IsFixed { get; set; }
        /// <summary>
        /// Indicated if this tile is empty and can be used to move atoms on this tile.
        /// </summary>
        /// <returns>True if tile is empty and can be played on it.</returns>
        public bool IsEmpty { get; set; }
        /// <summary>
        /// Constructs new instance of BoardTile class where tile is fixed by default.
        /// </summary>
        public BoardTile()
        {
            IsFixed = true;
        }
    }
}
