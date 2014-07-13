using Mach.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System;

namespace Mach.Xna.Kinect.Components
{
    public class RelativeCursorTracker : ICursorTracker
    {
        private KinectManager _kinectManager;
        private SkeletonPoint _previousPosition;
        private float _moveThreshold = 0.005f;

        public RelativeCursorTracker(KinectManager kinectManager)
        {
            _kinectManager = kinectManager;
            _moveThreshold = 0.005f;
        }

        public RelativeCursorTracker(KinectManager kinectManager, float moveThreshold)
        {
            _kinectManager = kinectManager;
            _moveThreshold = moveThreshold;
        }

        public Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height)
        {
            bool isHandTracked;

            return GetCursorPosition(skeleton, leftHanded, width, height, out isHandTracked);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/12569706/how-to-use-skeletal-joint-to-act-as-cursor-using-bounds-no-gestures
        /// Shoulders = top of screen
        /// Hips = bottom of screen
        /// Left Should = left most on screen
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="leftHanded"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="isHandTracked"></param>
        /// <returns></returns>
        public Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height, out bool isHandTracked)
        {
            isHandTracked = false;

            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint rightHand = skeleton.Joints[JointType.HandRight];

            Joint hand = leftHanded ? leftHand : rightHand;

            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];

            Joint centerShoulder = skeleton.Joints[JointType.ShoulderCenter];
            Joint oppositeShoulder = leftHanded ? rightShoulder : leftShoulder;
            Joint sameShoulder = leftHanded ? leftShoulder : rightShoulder;
            Joint hip = leftHanded ? skeleton.Joints[JointType.HipLeft] : skeleton.Joints[JointType.HipRight];

            Joint head = skeleton.Joints[JointType.Head];

            // the hand joint is tracked
            if (hand.TrackingState == JointTrackingState.Tracked)
            {
                // the hand is sufficiently in front of the shoulder
                if (sameShoulder.Position.Z - hand.Position.Z > 0.2)
                {
                    isHandTracked = true;

                    // the hand has moved enough to update screen position (jitter control / smoothing)
                    if (Math.Abs(hand.Position.X - _previousPosition.X) > _moveThreshold || Math.Abs(hand.Position.Y - _previousPosition.Y) > _moveThreshold)
                    {

                        float xScaled = (hand.Position.X - centerShoulder.Position.X) / ((sameShoulder.Position.X - centerShoulder.Position.X) * 2) * width;

                        if (leftHanded)
                            xScaled = width - xScaled;

                        float yScaled = _kinectManager.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated ?
                            (hand.Position.Y - sameShoulder.Position.Y) / ((sameShoulder.Position.Y - head.Position.Y) / 2) * height :
                            (hand.Position.Y - sameShoulder.Position.Y) / (hip.Position.Y - sameShoulder.Position.Y) * height;

                        if (yScaled < 0) yScaled = 0;
                        if (yScaled > height) yScaled = height;

                        if (xScaled < 0) xScaled = 0;
                        if (xScaled > width) xScaled = width;

                        _previousPosition = hand.Position;

                        return new Vector2(xScaled, yScaled);
                    }
                }
            }

            // As fallback return zero position.
            return Vector2.Zero;
        }
    }
}
