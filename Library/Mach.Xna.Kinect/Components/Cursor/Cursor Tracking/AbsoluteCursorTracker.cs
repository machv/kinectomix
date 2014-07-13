using System;
using Mach.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Maps absolutely cursor position to defined rectangle.
    /// </summary>
    public class AbsoluteCursorTracker : ICursorTracker
    {
        private KinectManager _kinectManager;

        /// <summary>
        /// Initializes a new instance of <see cref="AbsoluteCursorTracker"/> class.
        /// </summary>
        /// <param name="kinectManager">Manager handling connected Kinect sensor.</param>
        public AbsoluteCursorTracker(KinectManager kinectManager)
        {
            _kinectManager = kinectManager;
        }

        /// <summary>
        /// Gets mapped cursor position with respect to bounding limits based current <see cref="Skeleton"/> data.
        /// </summary>
        /// <param name="skeleton">Skeleton data to parse.</param>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="width">Width of the mapping area.</param>
        /// <param name="height">Height of the mapping area.</param>
        /// <returns>Mapped cursor position.</returns>
        public Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height)
        {
            bool isHandTracked;

            return GetCursorPosition(skeleton, leftHanded, width, height, out isHandTracked);
        }


        /// <summary>
        /// Gets mapped cursor position with respect to bounding limits based current <see cref="Skeleton"/> data.
        /// </summary>
        /// <param name="skeleton">Skeleton data to parse.</param>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="width">Width of the mapping area.</param>
        /// <param name="height">Height of the mapping area.</param>
        /// <param name="isHandTracked">Output parameter containing true if selected hand is active.</param>
        /// <returns>Mapped cursor position.</returns>
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
