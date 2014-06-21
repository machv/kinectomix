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
        public const string StorageContainerName = "State";
        private HighscoreData _data = new HighscoreData();
        private string _fileName;

        [XmlIgnore]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public string DefinitionHash
        {
            get { return _data.DefinitionHash; }
            set { _data.DefinitionHash = value; }
        }
        public LevelHighscore[] Levels
        {
            get { return _data.Levels; }
            set { _data.Levels = value; }
        }

        public static Highscore Load(string fileName)
        {
            Highscore highscore = null;

            StorageDevice device = GetStorageDevice();

            IAsyncResult result = _device.BeginOpenContainer(StorageContainerName, null, null);
            result.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = _device.EndOpenContainer(result))
            {
                if (container.FileExists(fileName))
                {
                    try
                    {
                        using (Stream stream = container.OpenFile(fileName, FileMode.Open))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Highscore));
                            highscore = (Highscore)serializer.Deserialize(stream);
                        }
                    }
                    catch
                    {
                        highscore = null;
                    }
                }
            }

            if (highscore == null)
                highscore = new Highscore();

            highscore.FileName = fileName;

            return highscore;
        }

        private static StorageDevice _device;

        private static StorageDevice GetStorageDevice()
        {
            if (_device == null || !_device.IsConnected)
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(null, null);

                result.AsyncWaitHandle.WaitOne();

                _device = StorageDevice.EndShowSelector(result);
            }

            return _device;
        }

        public bool Save()
        {
            StorageDevice device = GetStorageDevice();

            IAsyncResult result = _device.BeginOpenContainer(StorageContainerName, null, null);
            result.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = _device.EndOpenContainer(result))
            {
                result.AsyncWaitHandle.Close();

                if (container.FileExists(_fileName))
                    container.DeleteFile(_fileName);

                try
                {
                    using (Stream stream = container.CreateFile(_fileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Highscore));
                        serializer.Serialize(stream, this);
                    }
                }
                catch
                {
                    container.Dispose();
                    return false;
                }

            }

            return true;
        }
    }
}