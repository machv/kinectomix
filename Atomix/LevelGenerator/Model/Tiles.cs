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
        private List<BoardTile> _boardTiles = new List<BoardTile>();
        private List<BoardTile> _moleculeTiles = new List<BoardTile>();

        public IEnumerable<BoardTile> Board { get { return _boardTiles; } }

        public IEnumerable<BoardTile> Molecule { get { return _moleculeTiles; } }
    }
}
