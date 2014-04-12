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

        public Tiles()
        {
            foreach (var type in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                TilePropertiesAttribute properties = type.GetAttributeOfType<TilePropertiesAttribute>();
                bool isFixed = properties != null ? properties.IsFixed : false;

                if (properties != null && !properties.ShowInBoardEditor)
                    continue;

                _boardTiles.Add(new BoardTile() { Type = type, IsFixed = isFixed });
            }

            foreach (var type in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                TilePropertiesAttribute properties = type.GetAttributeOfType<TilePropertiesAttribute>();
                bool isFixed = properties != null ? properties.IsFixed : false;

                if (properties != null && !properties.ShowInMoleculeEditor)
                    continue;

                _moleculeTiles.Add(new BoardTile() { Type = type, IsFixed = isFixed });
            }
        }
    }
}
