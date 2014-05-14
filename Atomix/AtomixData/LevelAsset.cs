using System;
using System.Xml.Serialization;

namespace Kinectomix.Logic
{
    [Serializable]
    public class LevelAsset
    {
        public string AssetName { get;set; }

        public string AssetContent { get;set; }

        [XmlIgnore]
        public byte[] DecodedAssetContent
        {
            get
            {
                return System.Convert.FromBase64String(AssetContent);
            }
        }
    }
}
