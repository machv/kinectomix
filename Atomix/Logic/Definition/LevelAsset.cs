using System;
using System.Xml.Serialization;

namespace Mach.Kinectomix.Logic
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

        /// <summary>
        /// Gets or sets the name of the asset.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Gets or sets the code of the asset.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this asset has any rendered bond.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this asset has bonds; otherwise, <c>false</c>.
        /// </value>
        public bool HasBonds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this asset is fixed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fixed; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Gets the content of the decoded asset.
        /// </summary>
        /// <value>
        /// The content of the decoded asset.
        /// </value>
        [XmlIgnore]
        public byte[] DecodedAssetContent
        {
            get
            {
                return Convert.FromBase64String(Content);
            }
        }
    }
}
