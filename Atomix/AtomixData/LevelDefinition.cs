using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    public class LevelDefinition
    {
        public enum Type
        {
            Compiled,
            Serialized
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public Type LevelType { get; set; }

        public bool IsFinished { get; set; }

        public DateTime Finished { get; set; }
    }
}
