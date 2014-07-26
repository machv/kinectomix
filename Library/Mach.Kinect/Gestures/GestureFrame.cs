using Microsoft.Kinect;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Frame containing normalized positions of tracked joints for gesture.
    /// </summary>
    public class GestureFrame : Collection<SkeletonPoint>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GestureFrame"/> from recorded <see cref="FrameData"/>.
        /// </summary>
        /// <param name="frameData">Recorded data.</param>
        /// <param name="trackedJoints">Which points should be included in the gesture.</param>
        /// <param name="dimension">Dimension for tracked points.</param>
        /// <returns>Instance of <see cref="GestureFrame"/> created from recorded <see cref="FrameData"/>.</returns>
        public static GestureFrame FromFrameData(FrameData frameData, IEnumerable<JointType> trackedJoints, TrackingDimension dimension = TrackingDimension.Three)
        {
            GestureFrame frame = new GestureFrame();

            foreach (JointType joint in trackedJoints)
            {
                SkeletonPoint framePoint = frameData.SkeletonJoints[(int)joint];
                SkeletonPoint point = dimension == TrackingDimension.Two ?
                    new SkeletonPoint() { X = framePoint.X, Y = framePoint.Y } :
                    new SkeletonPoint() { X = framePoint.X, Y = framePoint.Y, Z = framePoint.Z };

                frame.Add(point);
            }

            return frame;
        }
    }
}
