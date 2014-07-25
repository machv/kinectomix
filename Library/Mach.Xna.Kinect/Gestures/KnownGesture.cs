using Mach.Kinect.Gestures;
using Microsoft.Xna.Framework.Content;

namespace Mach.Xna.Kinect.Gestures
{
    /// <summary>
    /// Represents known gesture for recognizing.
    /// </summary>
    public class KnownGesture
    {
        private string _fileName;
        private GestureType _type;
        private Gesture _instance;

        /// <summary>
        /// Gets or sets the name of the file that contains definition of the gesture.
        /// </summary>
        /// <value>
        /// The name of the file that contains definition of the gesture.
        /// </value>
        public string Filename
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        /// <summary>
        /// Gets or sets the type of the gesture.
        /// </summary>
        /// <value>
        /// The type of the gesture.
        /// </value>
        public GestureType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// Gets or sets the instance of the gesture loaded from definition file.
        /// </summary>
        /// <value>
        /// The instance of the gesture loaded from definition file.
        /// </value>
        [ContentSerializerIgnore]
        public Gesture Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }
    }
}
