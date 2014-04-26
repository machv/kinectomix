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
        private List<BoardTileViewModel> _boardTiles = new List<BoardTileViewModel>();
        private List<BoardTileViewModel> _moleculeTiles = new List<BoardTileViewModel>();

        public List<BoardTileViewModel> Board { get { return _boardTiles; } }

        public List<BoardTileViewModel> Molecule { get { return _moleculeTiles; } }
    }
}
