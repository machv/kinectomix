using Microsoft.Xna.Framework.Storage;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Manages saving high score information.
    /// </summary>
    public class Highscore
    {
        private string _fileName;
        private string _definitionHash;
        private LevelHighscore[] _levels;

        /// <summary>
        /// The storage container name in <see cref="StorageDevice"/>.
        /// </summary>
        public const string StorageContainerName = "State";

        /// <summary>
        /// Gets or sets the name of the file that contains serialized high score data.
        /// </summary>
        /// <value>
        /// The name of the file that contains serialized high score data..
        /// </value>
        [XmlIgnore]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        /// <summary>
        /// Gets or sets the definition hash for corresponding <see cref="GameDefinition"/>.
        /// </summary>
        /// <value>
        /// The definition hash for corresponding <see cref="GameDefinition"/>.
        /// </value>
        public string DefinitionHash
        {
            get { return _definitionHash; }
            set { _definitionHash = value; }
        }
        /// <summary>
        /// Gets or sets the high score of levels.
        /// </summary>
        /// <value>
        /// The high score of levels.
        /// </value>
        public LevelHighscore[] Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Highscore"/> class.
        /// </summary>
        public Highscore()
        {
            _levels = new LevelHighscore[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Highscore"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file that contains serialized high score data.</param>
        public Highscore(string fileName) : this()
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Gets the high score of level at specified index. This index corresponds with the index of the <see cref="Level"/> in <see cref="GameDefinition"/>.
        /// </summary>
        /// <param name="levelIndex">Index of the level.</param>
        /// <returns></returns>
        public LevelHighscore GetLevelHighscore(int levelIndex)
        {
            if (levelIndex >= _levels.Length || levelIndex < 0)
                return null;

            return _levels[levelIndex];
        }

        /// <summary>
        /// Sets the level high score of level at specified index. This index corresponds with the index of the <see cref="Level"/> in <see cref="GameDefinition"/>..
        /// </summary>
        /// <param name="levelIndex">Index of the level.</param>
        /// <param name="levelHighscore">The level high score.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">levelIndex</exception>
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

        /// <summary>
        /// Serializes this instance and saves it into the <see cref="FileName"/> file.
        /// </summary>
        /// <returns><c>true</c> if serialization succeeded.</returns>
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

        /// <summary>
        /// Deserializes the instance saved in the file.
        /// </summary>
        /// <param name="fileName">Name of the file that contains serialized instance of <see cref="Highscore"/> class.</param>
        /// <returns></returns>
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