using Mach.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System;
using Mach.Xna.Kinect.Extensions;
using Mach.Xna.Kinect.Components;

namespace Mach.Kinectomix
{
    /// <summary>
    /// Maps relatively cursor position to defined rectangle.
    /// </summary>
    public class VariableSpeedCursorMapper : ICursorMapper
    {
        private KinectManager _kinectManager;
        private SkeletonPoint _previousPosition;
        private float _moveThreshold = 0.01f;
        private Vector2 _previousCursor;
        private float _cursorSpeed;

        /// <summary>
        /// Gets or sets the cursor speed in percents. Compares current cursor position with previous and adds only defined percents of distance to cursor. 
        /// Value 0 means cursor will move slowly, Value 1 means normal speed without any modifications.
        /// </summary>
        /// <value>
        /// The cursor speed in percents.
        /// </value>
        public float CursorSpeed
        {
            get { return _cursorSpeed; }
            set { _cursorSpeed = value; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="VariableSpeedCursorMapper"/> class.
        /// </summary>
        /// <param name="kinectManager">Manager handling connected Kinect sensor.</param>
        public VariableSpeedCursorMapper(KinectManager kinectManager)
        {
            _kinectManager = kinectManager;
            _moveThreshold = 0.005f;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="VariableSpeedCursorMapper"/> class and sets movement threshold.
        /// </summary>
        /// <param name="kinectManager">Manager handling connected Kinect sensor.</param>
        /// <param name="moveThreshold">Minimal distance that has to be reached to return new cursor position.</param>
        public VariableSpeedCursorMapper(KinectManager kinectManager, float moveThreshold)
        {
            _kinectManager = kinectManager;
            _moveThreshold = moveThreshold;
            _cursorSpeed = 1;
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
        /// Limits:
        ///   * Shoulders = top of screen
        ///   * Hips = bottom of screen
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
                        float distance = Vector2.Distance(hand.Position.ToVector2(), _previousPosition.ToVector2());
                        //Vector2 difference = (hand.Position.ToVector2() - _previousPosition.ToVector2()) / 2;
                        Vector2 handPosition = hand.Position.ToVector2(); // - difference;

                        //System.Diagnostics.Debug.Print("distance: " + distance + " meters");

                        float xScaled = (handPosition.X - centerShoulder.Position.X) / ((sameShoulder.Position.X - centerShoulder.Position.X) * 2) * width;

                        if (leftHanded)
                            xScaled = width - xScaled;

                        float yScaled = _kinectManager.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated ?
                            (handPosition.Y - sameShoulder.Position.Y) / ((sameShoulder.Position.Y - head.Position.Y) / 2) * height :
                            (handPosition.Y - sameShoulder.Position.Y) / (hip.Position.Y - sameShoulder.Position.Y) * height;

                        Vector2 cursor = new Vector2(xScaled, yScaled);
                        cursor = SetBounds(width, height, cursor);

                        float distanceConstant = 0.1f;

                        Vector2 half = (cursor - _previousCursor) * (distanceConstant);
                        cursor = _previousCursor + half;

                        cursor = SetBounds(width, height, cursor);

                        _previousPosition = hand.Position;
                        _previousCursor = cursor;

                        return _previousCursor + half;
                    }
                }
            }

            // As fall back return zero position.
            return Vector2.Zero;
        }

        private static Vector2 SetBounds(int width, int height, Vector2 cursor)
        {
            if (cursor.Y < 0) cursor.Y = 0;
            if (cursor.X < 0) cursor.X = 0;
            if (cursor.X > width) cursor.X = width;
            if (cursor.Y > height) cursor.Y = height;
            return cursor;
        }
    }
}
