﻿using Microsoft.Xna.Framework;
using System;

namespace Kinectomix.Logic
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
        /// Constructs new instance of BoardTile class where tile is fixed by default.
        /// </summary>
        public BondType TopLeftBond { get; set; }
        public BondType TopBond { get; set; }
        public BondType TopRightBond { get; set; }
        public BondType RightBond { get; set; }
        public BondType BottomRightBond { get; set; }
        public BondType BottomBond { get; set; }
        public BondType BottomLeftBond { get; set; }
        public BondType LeftBond { get; set; }

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

        public BoardTile()
        {
            IsFixed = true;
        }

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