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
        private string _definitionHash;
        public string DefinitionHash
        {
            get { return _definitionHash; }
            set { _definitionHash = value; }
        }
        public LevelHighscore[] Levels { get; set; }
    }
}