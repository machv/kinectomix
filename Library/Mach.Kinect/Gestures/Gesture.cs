using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Gesture.
    /// </summary>
    [Serializable]
    public class Gesture
    {
        private TrackingDimension _dimension;
        private string _name;
        private int _id;
        private JointType[] _trackedJoints;
        private List<GestureFrame> _gestureSequence;

        /// <summary>
        /// Gets or sets dimension for tracked joints.
        /// </summary>
        /// <returns>Dimension for tracked joints.</returns>
        public TrackingDimension Dimension
        {
            get { return _dimension; }
            set { _dimension = value; }
        }
        /// <summary>
        /// Gets or sets friendly name of this gesture.
        /// </summary>
        /// <returns>Friendly name of this gesture.</returns>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Gets or sets user ID of this gesture.
        /// </summary>
        /// <returns>ID of this gesture.</returns>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Gets or sets list of joints required to observe for this gesture.
        /// </summary>
        /// <returns>List of joints required to observe for this gesture.</returns>
        public JointType[] TrackedJoints
        {
            get { return _trackedJoints; }
            set { _trackedJoints = value; }
        }
        /// <summary>
        /// Gets or sets list of invariant positions ordered by <see cref="TrackedJoints"/>.
        /// </summary>
        /// <returns>List of invariant positions ordered by <see cref="TrackedJoints"/>.</returns>
        public List<GestureFrame> Sequence
        {
            get { return _gestureSequence; }
            set { _gestureSequence = value; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Gesture"/> class.
        /// </summary>
        public Gesture()
        {
            _gestureSequence = new List<GestureFrame>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Gesture"/> class from recorded frames.
        /// </summary>
        /// <param name="frames">Recorded frames.</param>
        /// <param name="trackedJoints">Which joints should be used in the gesture.</param>
        /// <param name="dimension">Dimension for used joints in the gesture.</param>
        /// <returns>New initialized <see cref="Gesture"/>.</returns>
        public static Gesture FromFrameData(IEnumerable<FrameData> frames, IEnumerable<JointType> trackedJoints, TrackingDimension dimension = TrackingDimension.Three)
        {
            Gesture gesture = new Gesture();
            gesture.Dimension = dimension;
            gesture.TrackedJoints = trackedJoints.ToArray();

            foreach (FrameData frameData in frames)
                gesture.Sequence.Add(GestureFrame.FromFrameData(frameData, trackedJoints, dimension));

            return gesture;
        }
    }
}
