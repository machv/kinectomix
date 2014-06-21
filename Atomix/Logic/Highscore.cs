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

        private string _fileName;
        private string _definitionHash;
        private LevelHighscore[] _levels;

        [XmlIgnore]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public string DefinitionHash
        {
            get { return _definitionHash; }
            set { _definitionHash = value; }
        }
        public LevelHighscore[] Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }

        public Highscore()
        {
            _levels = new LevelHighscore[1];
        }

        public Highscore(string fileName) : this()
        {
            _fileName = fileName;
        }

        public LevelHighscore GetLevelHighscore(int levelIndex)
        {
            if (levelIndex >= _levels.Length || levelIndex < 0)
                throw new ArgumentOutOfRangeException("levelIndex");

            return _levels[levelIndex];
        }

        public void SetLevelHighscore(int levelIndex, LevelHighscore levelHighscore)
        {
            if (levelIndex < 0)
                throw new ArgumentOutOfRangeException("levelIndex");

            if (levelIndex >= _levels.Length)
            {
                LevelHighscore[] newLevels = new LevelHighscore[levelIndex + 1];
                Array.Copy(_levels, newLevels, _levels.Length);
                _levels = newLevels;
            }

            _levels[levelIndex] = levelHighscore;
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
    }
}