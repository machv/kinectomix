using System;

namespace Kinectomix.Logic
{
    [Serializable]
    public class GameState
    {
        public string DefinitionHash { get; set; }
        public Level[] Levels { get; set; }

        public int CurrentLevel { get; set; }

        public GameState() { }

        public GameState(Level[] levels)
        {
            Levels = levels;
        }

        public Level SwitchToNextLevel()
        {
            if (Levels.Length <= CurrentLevel + 1)
                return null;

            CurrentLevel += 1;

            return Levels[CurrentLevel];
        }

        public Level GetCurrentLevel()
        {
            return Levels[CurrentLevel];
        }

        public void SetLevelToCurrent(Level level)
        {
            int i = Array.IndexOf(Levels, level);

            CurrentLevel = i;
        }
    }
}
