﻿using Mach.Kinectomix.Logic;
using Microsoft.Xna.Framework;

namespace Mach.Kinectomix.ViewModel
{
    public class BoardTileViewModel
    {
        private BoardTile _tile;
        private float _opacity;

        public BoardTile Tile
        {
            get { return _tile; }
            set
            {
                _tile = value;
                InvalidateAssetCode();
            }
        }

        public string Asset
        {
            get { return _tile.Asset; }
            set
            {
                _tile.Asset = value;
                InvalidateAssetCode();
            }
        }

        public bool IsFixed
        {
            get { return _tile.IsFixed; }
            set { _tile.IsFixed = value; }
        }

        public float Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }

        public bool IsEmpty
        {
            get { return _tile.IsEmpty; }
            set { _tile.IsEmpty = value; }
        }

        string _assetCode = string.Empty;
        public string AssetCode
        {
            get { return _assetCode; }
        }

        /// <summary>
        /// Indicates if this tile is currently highlighted in the game.
        /// </summary>
        /// <returns>True if is hover should be active.</returns>
        public bool IsHovered { get; set; }

        /// <summary>
        /// Indicates if this tile is currently active in the game.
        /// </summary>
        /// <returns>True if is selected.</returns>
        public bool IsSelected { get; set; }
        /// <summary>
        /// Flag indicating in which directions can be this tile moved on the board.
        /// </summary>
        public MoveDirection Movements { get; set; }

        /// <summary>
        /// Render position of this tile on the board.
        /// </summary>
        public Vector2 RenderPosition { get; set; }

        /// <summary>
        /// Gets or sets the scale for rendering this tile.
        /// </summary>
        /// <value>
        /// The scale for rendering this tile.
        /// </value>
        public float RenderScale { get; set; }

        /// <summary>
        /// Rendering rectangle in which tile is on the board. Used for hit testing.
        /// </summary>
        /// <returns></returns>
        public Rectangle RenderRectangle { get; set; }
        /// <summary>
        /// Gets or sets the width of the tile.
        /// </summary>
        /// <value>
        /// The width of the render.
        /// </value>
        //public int RenderWidth { get; set; }
        /// <summary>
        /// Gets or sets the height of the tile.
        /// </summary>
        /// <value>
        /// The height of the render.
        /// </value>
        //public int RenderHeight { get; set; }

        public BoardTileViewModel(BoardTile tile) :this()
        {
            _tile = tile;
            InvalidateAssetCode();
        }

        protected void InvalidateAssetCode()
        {
            _assetCode = GetAssetCode(_tile);
        }

        public BoardTileViewModel()
        {
            _opacity = 1;
        }

        public static string GetAssetCode(BoardTile tile)
        {
            // System assets are unique
            switch (tile.Asset)
            {
                case "Up":
                case "Down":
                case "Left":
                case "Right":
                case "Empty":
                    return tile.Asset;
            }

            return !tile.HasBonds ? tile.Asset :
                                    string.Format("{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}_{0}", tile.Asset,
                                        (int)tile.TopLeftBond,
                                        (int)tile.TopBond,
                                        (int)tile.TopRightBond,
                                        (int)tile.RightBond,
                                        (int)tile.BottomRightBond,
                                        (int)tile.BottomBond,
                                        (int)tile.BottomLeftBond,
                                        (int)tile.LeftBond);
        }
    }
}
