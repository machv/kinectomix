using Atomix.ViewModel;
using Kinectomix.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Atomix
{
    public static class LevelFactory
    {
        public static Level Load(string path)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Level));

            using (Stream stream = TitleContainer.OpenStream(path))
                return seralizer.Deserialize(stream) as Level;
        }
    }
}
