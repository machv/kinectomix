using Microsoft.Xna.Framework;
using System;

namespace Mach.Kinectomix.Logic
{
    [Serializable]
    /// <summary>
    /// Represents one tile on the game's board.
    /// </summary>
    public class BoardTile
    {
        /// <summary>
        /// User friendly name of current tile. 
        /// </summary>
        /// <returns>Name of current tile.</returns>
        public string Name { get; set; }
        /// <summary>
        /// Image asset representing this tile, this is key used to look up in the level assets.
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
        /// Gets or sets the top left bond.
        /// </summary>
        /// <value>
        /// The top left bond.
        /// </value>
        public BondType TopLeftBond { get; set; }
        /// <summary>
        /// Gets or sets the top bond.
        /// </summary>
        /// <value>
        /// The top bond.
        /// </value>
        public BondType TopBond { get; set; }
        /// <summary>
        /// Gets or sets the top right bond.
        /// </summary>
        /// <value>
        /// The top right bond.
        /// </value>
        public BondType TopRightBond { get; set; }
        /// <summary>
        /// Gets or sets the right bond.
        /// </summary>
        /// <value>
        /// The right bond.
        /// </value>
        public BondType RightBond { get; set; }
        /// <summary>
        /// Gets or sets the bottom right bond.
        /// </summary>
        /// <value>
        /// The bottom right bond.
        /// </value>
        public BondType BottomRightBond { get; set; }
        /// <summary>
        /// Gets or sets the bottom bond.
        /// </summary>
        /// <value>
        /// The bottom bond.
        /// </value>
        public BondType BottomBond { get; set; }
        /// <summary>
        /// Gets or sets the bottom left bond.
        /// </summary>
        /// <value>
        /// The bottom left bond.
        /// </value>
        public BondType BottomLeftBond { get; set; }
        /// <summary>
        /// Gets or sets the left bond.
        /// </summary>
        /// <value>
        /// The left bond.
        /// </value>
        public BondType LeftBond { get; set; }

        /// <summary>
        /// Gets a value indicating whether this tile has any bond.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this tile has bonds; otherwise, <c>false</c>.
        /// </value>
        public bool HasBonds
        {
            get
            {
                return BottomBond > 0 ||
                    BottomLeftBond > 0 ||
                    BottomRightBond > 0 ||
                    LeftBond > 0 ||
                    RightBond > 0 ||
                    TopBond > 0 ||
                    TopLeftBond > 0 ||
                    TopRightBond > 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardTile"/> class.
        /// </summary>
        public BoardTile()
        {
            IsFixed = true;
        }

        /// <summary>
        /// Gets the asset code.
        /// </summary>
        /// <returns></returns>
        public string GetAssetCode()
        {
            return string.Format("{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}_{0}", Asset,
                (int)TopLeftBond,
                (int)TopBond,
                (int)TopRightBond,
                (int)RightBond,
                (int)BottomRightBond,
                (int)BottomBond,
                (int)BottomLeftBond,
                (int)LeftBond);
        }
    }
}
