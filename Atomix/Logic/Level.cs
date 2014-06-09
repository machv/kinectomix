using System;
using System.Collections.Generic;

namespace Kinectomix.Logic
{
    [Serializable]
    public class Level
    {
        public List<LevelAsset> Assets { get; set; }

        /// <summary>
        /// Gets or sets friendly name of the level.
        /// </summary>
        /// <returns>Friendly name of the level.</returns>
        public string Name { get; set; }

        public TilesCollection<BoardTile> Board { get; set; }
        public TilesCollection<BoardTile> Molecule { get; set; }

        public Level()
        {
            Assets = new List<LevelAsset>();
        }
    }
}
