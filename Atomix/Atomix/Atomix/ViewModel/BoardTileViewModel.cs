using Kinectomix.Logic;
using Microsoft.Xna.Framework;

namespace Atomix.ViewModel
{
    public class BoardTileViewModel
    {
        BoardTile _tile;

        public BoardTile Tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public string Asset
        {
            get { return _tile.Asset; }
            set { _tile.Asset = value; }
        }

        public bool IsFixed
        {
            get { return _tile.IsFixed; }
            set { _tile.IsFixed = value; }
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
        /// Indicates if this tile is currently active in the game.
        /// </summary>
        /// <returns>True if is selected.</returns>
        public bool IsSelected { get; set; }
        /// <summary>
        /// Flag indicating in which directions can be this tile moved on the board.
        /// </summary>
        public Direction Movements { get; set; }

        /// <summary>
        /// Render position of this tile on the board.
        /// </summary>
        public Vector2 RenderPosition { get; set; }

        public BoardTileViewModel(BoardTile tile)
        {
            _tile = tile;
            _assetCode = GetAssetCode(tile);
        }

        public BoardTileViewModel() { }

        public static string GetAssetCode(BoardTile tile)
        {
            return string.Format("{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}_{0}", tile.Asset,
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
