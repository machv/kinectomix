using System;
using System.Collections.Generic;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Definition of one game level.
    /// </summary>
    [Serializable]
    public class Level
    {
        private List<LevelAsset> _assets;
        private string _name;
        private TilesCollection<BoardTile> _board;
        private TilesCollection<BoardTile> _molecule;

        /// <summary>
        /// Gets or sets list of binary assets used in the level.
        /// </summary>
        /// <returns>List of assets used in the level.</returns>
        public List<LevelAsset> Assets
        {
            get { return _assets; }
            set { _assets = value; }
        }
        /// <summary>
        /// Gets or sets friendly name of the level.
        /// </summary>
        /// <returns>Friendly name of the level.</returns>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Gets or sets <see cref="TilesCollection{T}"/> containing game tiles.
        /// </summary>
        /// <returns>Game tiles collection.</returns>
        public TilesCollection<BoardTile> Board
        {
            get { return _board; }
            set { _board = value; }
        }
        /// <summary>
        /// Gets or sets <see cref="TilesCollection{T}"/> containing tiles describing molecule definition.
        /// </summary>
        /// <returns>Molecule definition.</returns>
        public TilesCollection<BoardTile> Molecule
        {
            get { return _molecule; }
            set { _molecule = value; }
        }

        /// <summary>
        /// Initializes new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            Assets = new List<LevelAsset>();
        }
    }
}
