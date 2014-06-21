using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    [Serializable]
    public class LevelHighscore
    {
        public TimeSpan Time { get; set; }
        public int Moves { get; set; }
        public DateTime When { get; set; }
    }
}
