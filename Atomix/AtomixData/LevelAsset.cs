using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KinectomixLogic
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
