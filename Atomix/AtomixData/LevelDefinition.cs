using Microsoft.Xna.Framework.Content;
using System;

namespace Kinectomix.Logic
{
    public class LevelDefinition
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public bool IsFinished { get; set; }

        public DateTime Finished { get; set; }
        
        [ContentSerializerIgnore]
        public string AssetName { get { return System.IO.Path.GetFileNameWithoutExtension(FileName); } }
    }
}
