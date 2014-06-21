using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AtomixData
{
    public class Highscore
    {
        private HighscoreData _data;
        private string _path;
        public string Path
        {
            get { return _path; }
        }

        public string DefinitionHash
        {
            get { return _data.DefinitionHash; }
            set { _data.DefinitionHash = value; }
        }
        public List<LevelHighscore> Levels
        {
            get { return _data.Levels; }
            set { _data.Levels = value; }
        }

        public Highscore(string file)
        {
            _path = file;

            Load();

            if (_data == null)
            {
                _data = new HighscoreData();
            }
        }

        private HighscoreData Load()
        {
            using (Stream stream = TitleContainer.OpenStream(_path))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                    _data = (HighscoreData)serializer.Deserialize(stream);
                }
                catch
                {
                    return null;
                }

                return _data;
            }
        }

        public bool Save()
        {
            using (Stream stream = TitleContainer.OpenStream(_path))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                    serializer.Serialize(stream, _data);
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
    }
}