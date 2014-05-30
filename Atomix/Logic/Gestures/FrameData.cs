using Microsoft.Kinect;

namespace Kinectomix.Logic.Gestures
{
    public class FrameData
    {
        public SkeletonPoint[] SkeletonJoints { get; set; }

        public int Capacity { get; private set; }

        public FrameData(int capacity)
        {
            Capacity = capacity;
            SkeletonJoints = new SkeletonPoint[capacity];
        }
    }
}
