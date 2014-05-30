using System;
using System.Collections.Generic;

namespace Kinectomix.Logic
{
    [Serializable]
    public class Level
    {
        public List<LevelAsset> Assets { get; set; }

        public string Name { get; set; }

        public TilesCollection<BoardTile> Board { get; set; }
        public TilesCollection<BoardTile> Molecule { get; set; }

        public Level()
        {
            Assets = new List<LevelAsset>();
        }
    }
}
