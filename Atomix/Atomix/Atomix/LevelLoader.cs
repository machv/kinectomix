using AtomixData;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Atomix
{
    public static class LevelLoader
    {
        public static Level Load(string path)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Level));

            using (Stream stream = TitleContainer.OpenStream(path))
                return seralizer.Deserialize(stream) as Level;
        }
    }
}
