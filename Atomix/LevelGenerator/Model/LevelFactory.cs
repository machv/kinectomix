using AtomixData;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kinectomix.LevelGenerator.Model
{
    public class LevelFactory
    {
        public static Level Load(string path)
        {
            if (System.IO.Path.GetExtension(path).ToLower() == ".xnb")
                return LoadFromCompiled(path);
            else if (System.IO.Path.GetExtension(path).ToLower() == ".xml")
                return LoadFromDefinition(path);
            else
                throw new ArgumentException("Unexpected file definition extension.");
        }

        public static Level LoadFromDefinition(string path)
        {
            using (XmlReader reader = XmlReader.Create(path))
                return IntermediateSerializer.Deserialize<AtomixData.Level>(reader, null);
        }

        public static Level LoadFromCompiled(string path)
        {
            Microsoft.Xna.Framework.Content.ContentManager cm = new Microsoft.Xna.Framework.Content.ContentManager(new Kinectomix.LevelGenerator.DummyServiceProvider());
            cm.RootDirectory = System.IO.Path.GetDirectoryName(path);

            return cm.Load<AtomixData.Level>(System.IO.Path.GetFileNameWithoutExtension(path));
        }
    }
}
