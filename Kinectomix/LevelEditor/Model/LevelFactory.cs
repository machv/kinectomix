using Mach.Kinectomix.Logic;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Mach.Kinectomix.LevelEditor.Model
{
    /// <summary>
    /// Loads and saves level definition files.
    /// </summary>
    public class LevelFactory
    {
        /// <summary>
        /// Loads the level definition from specified path.
        /// </summary>
        /// <param name="path">The path to level definition.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unexpected file definition extension.</exception>
        public static Level Load(string path)
        {
            if (Path.GetExtension(path).ToLower() == ".atxl")
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

        /// <summary>
        /// Saves the level definition to file.
        /// </summary>
        /// <param name="level">The level to save.</param>
        /// <param name="stream">The stream to which save data.</param>
        public static void SaveLevelXmlSerialized(Level level, Stream stream)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Level));
            seralizer.Serialize(stream, level);
        }
    }
}
