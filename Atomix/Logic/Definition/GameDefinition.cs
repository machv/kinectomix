using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mach.Kinectomix.Logic
{
    [Serializable]
    public class GameDefinition
    {
        private string _hash;
        private int _levelIndex;
        private Level[] _levels;
        public Level[] Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
        public string Hash
        {
            get { return _hash; }
            set { _hash = value; }
        }
        public GameDefinition(int levelsCount)
        {
            _levels = new Level[levelsCount];
        }

        public GameDefinition()
        {

        }

        public void AddLevel(Level level)
        {
            _levels[_levelIndex++] = level;
        }
    }
}
