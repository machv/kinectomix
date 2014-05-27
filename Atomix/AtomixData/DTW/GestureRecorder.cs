using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Kinectomix.Logic.DTW
{
    public class GestureRecorder : GestureProcessor
    {
        private IEnumerable<JointType> _trackedJoints;
        private GestureTrackingDimension _dimension;

        public void Start(IEnumerable<JointType> trackedJoints, GestureTrackingDimension dimension)
        {
            Clear();

            _minimalBufferLength = int.MaxValue;
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
