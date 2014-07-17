using Mach.Kinectomix.Logic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace Mach.Kinectomix.LevelEditor.Model
{
    public class LevelFactory
    {
        public static Level Load(string path)
        {
            if (System.IO.Path.GetExtension(path).ToLower() == ".xnb")
                return LoadFromCompiled(path);
            else if (System.IO.Path.GetExtension(path).ToLower() == ".xml")
                return LoadFromDefinition(path);
            else if (System.IO.Path.GetExtension(path).ToLower() == ".atb")
                return LoadFromBinary(path);
            else if (System.IO.Path.GetExtension(path).ToLower() == ".atxl")
                return LoadFromXmlSerialized(path);
            else
                throw new ArgumentException("Unexpected file definition extension.");
        }

        private static Level LoadFromXmlSerialized(string path)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Level));

            using (Stream stream = File.Open(path, FileMode.Open))
                return seralizer.Deserialize(stream) as Level;
        }

        public static Level LoadFromBinary(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (Stream stream = File.Open(path, FileMode.Open))
                return bf.Deserialize(stream) as Level;
        }

        public static Level LoadFromDefinition(string path)
        {
            using (XmlReader reader = XmlReader.Create(path))
                return IntermediateSerializer.Deserialize<Level>(reader, null);
        }

        public static Level LoadFromCompiled(string path)
        {
            Microsoft.Xna.Framework.Content.ContentManager cm = new Microsoft.Xna.Framework.Content.ContentManager(new Mach.Kinectomix.Logic.DummyServiceProvider());
            cm.RootDirectory = Path.GetDirectoryName(path);

            return cm.Load<Level>(Path.GetFileNameWithoutExtension(path));
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

        public static void SaveLevelBinary(Level level, Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, level);
        }

        public static void SaveLevelXmlSerialized(Level level, Stream stream)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Level));
            seralizer.Serialize(stream, level);
        }
    }
}
