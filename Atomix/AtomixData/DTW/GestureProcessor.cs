using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic.DTW
{
    public class GestureProcessor
    {
        public const int TrackedJointsCount = 20;

        Queue<FrameData> FrameBuffer;

        private static JointType[] _tracableSkeletonJoints;

        static GestureProcessor()
        {
            _tracableSkeletonJoints = (JointType[])Enum.GetValues(typeof(JointType));
        }

        public void AddGesture(Gesture gesture)
        {

        }

        public void ProcessSkeleton(Skeleton skeleton)
        {
            // normalizovat body
            //skeleton.Joints

            FrameData frame = new FrameData(TrackedJointsCount);

            foreach(JointType joint in _tracableSkeletonJoints)
                frame.SkeletonJoints[(int)joint] = new SkeletonPoint() { X = 0, Y = 0, Z = 0 };
        }
    }
}
