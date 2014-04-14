using AtomixData;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static void SaveLevelDefinition(Level level, Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                IntermediateSerializer.Serialize(writer, level, null);
            }
        }

        public static void SaveLevelCompiled(Level level, Stream stream, string fileName)
        {
            // http://stackoverflow.com/questions/8856528/serialize-texture2d-programatically-in-xna
            Type compilerType = typeof(ContentCompiler);
            ContentCompiler cc = compilerType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(null) as ContentCompiler;
            var compileMethod = compilerType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Instance);

            compileMethod.Invoke(cc, new object[]{
                                          stream, level, TargetPlatform.Windows, GraphicsProfile.Reach, false/*true*/, fileName, fileName
                                          });
        }
    }
}
