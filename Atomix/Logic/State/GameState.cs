using System;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Represents state of the game.
    /// </summary>
    [Serializable]
    public class GameState
    {
        private Highscore _highscore;
        private string _definitionHash;
        private Level[] _levels;
        private int _currentLevel;

        /// <summary>
        /// Gets or sets the high score manager.
        /// </summary>
        /// <value>
        /// The high score manager.
        /// </value>
        public Highscore Highscore
        {
            get { return _highscore; }
            set { _highscore = value; }
        }
        /// <summary>
        /// Gets or sets the definition hash of the <see cref="GameDefinition"/> that corresponds with this state.
        /// </summary>
        /// <value>
        /// The definition hash.
        /// </value>
        public string DefinitionHash
        {
            get { return _definitionHash; }
            set { _definitionHash = value; }
        }
        /// <summary>
        /// Gets or sets the levels for this state.
        /// </summary>
        /// <value>
        /// The levels for this state.
        /// </value>
        public Level[] Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
        /// <summary>
        /// Gets or sets the index of the currently played level.
        /// </summary>
        /// <value>
        /// The index of the currently played level.
        /// </value>
        public int CurrentLevel
        {
            get { return _currentLevel; }
            set { _currentLevel = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        public GameState() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        /// <param name="levels">The levels for this state.</param>
        public GameState(Level[] levels)
        {
            _levels = levels;
        }

        /// <summary>
        /// Updates to the next level.
        /// </summary>
        /// <returns>Next <see cref="Level"/> in the game.</returns>
        public Level SwitchToNextLevel()
        {
            if (_levels.Length <= _currentLevel + 1)
                return null;

            _currentLevel += 1;

            return _levels[CurrentLevel];
        }

        /// <summary>
        /// Gets the current level.
        /// </summary>
        /// <returns>The <see cref="Level"/> definition.</returns>
        public Level GetCurrentLevel()
        {
            return _levels[CurrentLevel];
        }

        /// <summary>
        /// Gets the high score for the current level.
        /// </summary>
        /// <returns>The high score for the current level.</returns>
        public LevelHighscore GetCurrentLevelHighscore()
        {
            if (_highscore == null)
                return null;

            return _highscore.GetLevelHighscore(CurrentLevel);
        }

        /// <summary>
        /// Sets the high score for the current level.
        /// </summary>
        /// <param name="levelHighscore">The high score for current level.</param>
        public void SetCurrentLevelHighscore(LevelHighscore levelHighscore)
        {
            if (_highscore == null)
                return;

            _highscore.SetLevelHighscore(CurrentLevel, levelHighscore);
        }

        /// <summary>
        /// Sets the specified level as current.
        /// </summary>
        /// <param name="level">The level.</param>
        public void SetLevelToCurrent(Level level)
        {
            int i = Array.IndexOf(Levels, level);

            _currentLevel = i;
        }
    }
}
