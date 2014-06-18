using System;
using System.Xml.Serialization;

namespace Kinectomix.Logic
{
    /// <summary>
    /// Contains base64 coded asset used in the game level.
    /// </summary>
    [Serializable]
    public class LevelAsset
    {
        private string _name;
        private string _code;
        private string _content;
        private bool _hasBonds;
        private bool _isFixed;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        public string AssetContent { get; set; }

        public bool HasBonds { get; set; }

        public bool IsFixed { get; set; }

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
