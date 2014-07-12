using Microsoft.Kinect;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Stores recorded joints for one frame.
    /// </summary>
    public class FrameData
    {
        private SkeletonPoint[] _skeletonJoints;

        /// <summary>
        /// Gets saved point coordinates for skeleton joints.
        /// </summary>
        /// <returns>Saved point coordinates for skeleton joints.</returns>
        public SkeletonPoint[] SkeletonJoints
        {
            get { return _skeletonJoints; }
        }

        /// <summary>
        /// Initializes new instance of <see cref="FrameData"/> class with defined skeleton joints capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public FrameData(int capacity)
        {
            _skeletonJoints = new SkeletonPoint[capacity];
        }
    }
}
