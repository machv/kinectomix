using System;
using Mach.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Kinect.Components
{
    public class AbsoluteCursorTracker : ICursorTracker
    {
        private KinectManager _kinectManager;

        public AbsoluteCursorTracker(KinectManager kinectManager)
        {
            _kinectManager = kinectManager;
        }
        public Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height)
        {
            bool isHandTracked;

            return GetCursorPosition(skeleton, leftHanded, width, height, out isHandTracked);
        }

        public Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height, out bool isHandTracked)
        {
            isHandTracked = false;

            JointType hand = leftHanded ? JointType.HandLeft : JointType.HandRight;
            SkeletonPoint handPoint = skeleton.Joints[hand].Position;

            // the hand joint is tracked
            if (skeleton.Joints[hand].TrackingState == JointTrackingState.Tracked)
            {
                isHandTracked = true;

                DepthImagePoint point = _kinectManager.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(handPoint, _kinectManager.Sensor.DepthStream.Format);
                double ratioX = (double)point.X / _kinectManager.Sensor.ColorStream.FrameWidth;
                double ratioY = (double)point.Y / _kinectManager.Sensor.ColorStream.FrameHeight;

                Vector2 position = new Vector2();
                position.X = (int)(width * ratioX);
                position.Y = (int)(height * ratioY);

                return position;
            }

            // As fallback return zero position.
            return Vector2.Zero;
        }
    }
}
