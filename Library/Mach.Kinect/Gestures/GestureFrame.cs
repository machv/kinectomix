using Microsoft.Kinect;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mach.Kinect.Gestures
{
    public class GestureFrame : Collection<SkeletonPoint>
    {
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
