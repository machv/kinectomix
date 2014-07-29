using System;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Defines all levels for the game.
    /// </summary>
    [Serializable]
    public class GameDefinition
    {
        private string _hash;
        private int _levelIndex;
        private Level[] _levels;

        /// <summary>
        /// Gets or sets game levels.
        /// </summary>
        /// <value>
        /// Game levels.
        /// </value>
        public Level[] Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
        /// <summary>
        /// Gets or sets the hash for comparing multiple definitions.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public string Hash
        {
            get { return _hash; }
            set { _hash = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDefinition"/> class.
        /// </summary>
        /// <param name="levelsCount">The levels count.</param>
        public GameDefinition(int levelsCount)
        {
            _levels = new Level[levelsCount];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDefinition"/> class.
        /// </summary>
        public GameDefinition() { }

        /// <summary>
        /// Adds new level to the definition.
        /// </summary>
        /// <param name="level">The level to add.</param>
        public void AddLevel(Level level)
        {
            _levels[_levelIndex++] = level;
        }
    }
}
