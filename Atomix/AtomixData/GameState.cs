using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectomixLogic
{
    [Serializable]
    public class GameState
    {
        public LevelDefinition[] Levels { get; set; }

        public int CurrentLevel { get; set; }

        public GameState() { }

        public GameState(LevelDefinition[] levels)
        {
            Levels = levels;
        }

        public LevelDefinition SwitchToNextLevel()
        {
            if (Levels.Length <= CurrentLevel + 1)
                return null;

            CurrentLevel += 1;

            return Levels[CurrentLevel];
        }

        public LevelDefinition GetCurrentLevel()
        {
            return Levels[CurrentLevel];
        }

        public void SetLevelToCurrent(LevelDefinition level)
        {
            int i = Array.IndexOf(Levels, level);

            CurrentLevel = i;
        }
    }
}
