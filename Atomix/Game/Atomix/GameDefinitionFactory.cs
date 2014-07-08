using Kinectomix.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Atomix
{
    public static class GameDefinitionFactory
    {
        public static readonly string StorageContainerName = "Game";
        public static readonly string TitlePath = "Content/";
        public static readonly string DefinitionFileName = "Definition.atx";

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

        public static GameDefinition Load()
        {
            GameDefinition definition = LoadFromUserStorage();
            if (definition == null)
                definition = LoadFromTitleStorage();

            return definition;
        }

        private static GameDefinition LoadFromStream(Stream stream)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(GameDefinition));

            using (var sha1 = SHA1.Create())
            {
                stream.Seek(0, SeekOrigin.Begin);

                byte[] rawHash = sha1.ComputeHash(stream);
                string definitionHash = Convert.ToBase64String(rawHash);

                stream.Seek(0, SeekOrigin.Begin);

                GameDefinition definition = seralizer.Deserialize(stream) as GameDefinition;
                definition.Hash = definitionHash;

                return definition;
            }
        }

        public static GameDefinition LoadFromTitleStorage()
        {
            try
            {
                using (Stream stream = TitleContainer.OpenStream(string.Format("{0}/{1}", TitlePath, DefinitionFileName)))
                    return LoadFromStream(stream);
            }
            catch
            {
                return null;
            }
        }

        public static GameDefinition LoadFromUserStorage()
        {
            StorageDevice device = GetStorageDevice();

            IAsyncResult result = _device.BeginOpenContainer(StorageContainerName, null, null);
            result.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = _device.EndOpenContainer(result))
            {
                if (container.FileExists(DefinitionFileName))
                {
                    try
                    {
                        using (Stream stream = container.OpenFile(DefinitionFileName, FileMode.Open))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(GameDefinition));
                            return LoadFromStream(stream);
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return null;
        }
    }
}
