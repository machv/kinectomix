using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AtomixData
{
    [Serializable]
    public class HighscoreData
    {
        public List<LevelHighscore> Highscores { get; set; }
    }
}