using KinectomixLogic;
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
        }

        public BoardTileViewModel() { }
    }
}
