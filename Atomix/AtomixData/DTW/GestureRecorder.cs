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

            _trackedJoints = trackedJoints;
            _dimension = dimension;
        }

        public Gesture GetRecordedGesture()
        {
            Gesture gesture = new Gesture();
            gesture.TrackedJoints = _trackedJoints.ToArray();
            gesture.Dimension = _dimension;
            gesture.GestureSequence = new List<double[]>();

            
            foreach (FrameData frame in _frameBuffer)
            {
                double[] frameData = new double[(int)_dimension * _trackedJoints.Count()];

                int i = 0;
                foreach (JointType joint in _trackedJoints)
                {
                    frameData[i++] = frame.SkeletonJoints[(int)joint].X;
                    frameData[i++] = frame.SkeletonJoints[(int)joint].Y;

                   if (_dimension == GestureTrackingDimension.Three)
                        frameData[i++] = frame.SkeletonJoints[(int)joint].Z;
                }

                gesture.GestureSequence.Add(frameData);
            }

            return gesture;
        }

        public override void ProcessSkeleton(Skeleton skeleton)
        {
            base.ProcessSkeleton(skeleton);
        }
    }
}
