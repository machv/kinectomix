using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic
{
    public class GameLevels
    {
        private List<Level> _levels;
        public List<Level> Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
    }
}
