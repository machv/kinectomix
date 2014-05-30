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
        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private HighscoreData _data;
        public HighscoreData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Highscore(string file)
        {
            _path = file;
        }

        public HighscoreData Load()
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