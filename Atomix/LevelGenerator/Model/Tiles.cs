using AtomixData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.Model
{
    public class Tiles
    {
        public enum TileType
        {
            Board,
            Molecule,
        }

        private List<BoardTileViewModel> _tiles = new List<BoardTileViewModel>();
        private List<BoardTileViewModel> _boardTiles = new List<BoardTileViewModel>();
        private List<BoardTileViewModel> _moleculeTiles = new List<BoardTileViewModel>();
        private Dictionary<string, TileAsset> _assets = new Dictionary<string, TileAsset>();

        public List<BoardTileViewModel> Items { get { return _tiles; } }

        public List<BoardTileViewModel> Board { get { return _boardTiles; } }

        public List<BoardTileViewModel> Molecule { get { return _moleculeTiles; } }

        public Dictionary<string, TileAsset> Assets { get { return _assets; } }

        public void Add(BoardTileViewModel tile, TileType destination)
        {
            if (!_tiles.Contains(tile))
            {
                _tiles.Add(tile);
            }

            switch (destination)
            {
                case TileType.Board:
                    _boardTiles.Add(tile);
                    break;
                case TileType.Molecule:
                    _moleculeTiles.Add(tile);
                    break;
            }
        }

        public BoardTileViewModel this[string name]
        {
            get { return _tiles.Where(t => t.Asset == name).FirstOrDefault(); }
        }
    }
}
