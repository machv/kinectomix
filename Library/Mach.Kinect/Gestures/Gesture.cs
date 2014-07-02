using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Collections.ObjectModel;

namespace Mach.Kinect.Gestures
{
    [Serializable]
    public class Gesture
    {
        public TrackingDimension Dimension { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        /// <summary>
        /// List of joints required to observe for this gesture.
        /// </summary>
        /// <returns></returns>
        public JointType[] TrackedJoints { get; set; }

        private List<GestureFrame> _gestureSequence = new List<GestureFrame>();
        /// <summary>
        /// List of invariant positions ordered by TrackedJoints
        /// </summary>
        /// <returns></returns>
        public List<GestureFrame> Sequence
        {
            get { return _gestureSequence; }
            set { _gestureSequence = value; }
        }

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
