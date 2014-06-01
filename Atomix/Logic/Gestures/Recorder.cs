using System.Collections.Generic;
using Microsoft.Kinect;

namespace Kinectomix.Logic.Gestures
{
    public class Recorder : GestureProcessor
    {
        private IEnumerable<JointType> _trackedJoints;
        private TrackingDimension _dimension;

        public void Start(IEnumerable<JointType> trackedJoints, TrackingDimension dimension)
        {
            Clear();

            _maximalBufferLength = int.MaxValue;
            _trackedJoints = trackedJoints;
            _dimension = dimension;
        }

        public Gesture GetRecordedGesture()
        {
            return Gesture.FromFrameData(_frameBuffer, _trackedJoints, _dimension);
        }

        public override void ProcessSkeleton(Skeleton skeleton)
        {
            base.ProcessSkeleton(skeleton);
        }
    }
}
