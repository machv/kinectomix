using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace Mach.Kinect.Gestures
{
    public abstract class GestureProcessor
    {
        public const int TrackedJointsCount = 20;

        protected static JointType[] _tracableSkeletonJoints;

        protected Queue<FrameData> _frameBuffer;
        protected int _maximalBufferLength = 30;

        /// <summary>
        /// Last processed frame inserted into the frame buffer.
        /// </summary>
        protected FrameData _lastFrame;


        public int MaximalBufferLength
        {
            get { return _maximalBufferLength; }
            set { if (value > 0) _maximalBufferLength = value; }
        }
        public int FrameBufferLength
        {
            get { return _frameBuffer.Count; }
        }

        static GestureProcessor()
        {
            _tracableSkeletonJoints = (JointType[])Enum.GetValues(typeof(JointType));
        }

        public GestureProcessor()
        {
            _frameBuffer = new Queue<FrameData>(_maximalBufferLength);
        }

        public void Clear()
        {
            _frameBuffer.Clear();
        }

        public virtual void ProcessSkeleton(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                if (_frameBuffer.Count > 0)
                    _frameBuffer.Dequeue();

                return;
            }

            // transpozice and normalizace of coordinates, 1 unit is length of shoulders (ShoulderRight - ShoulderLeft) 
            JointType first = JointType.Head;
            JointType second = JointType.ShoulderCenter;

            if (skeleton.Joints[first].TrackingState != JointTrackingState.Tracked ||
                skeleton.Joints[second].TrackingState != JointTrackingState.Tracked)
                return;

            SkeletonPoint center = new SkeletonPoint()
            {
                X = (skeleton.Joints[second].Position.X + skeleton.Joints[first].Position.X) / 2,
                Y = (skeleton.Joints[second].Position.Y + skeleton.Joints[first].Position.Y) / 2,
                Z = (skeleton.Joints[second].Position.Z + skeleton.Joints[first].Position.Z) / 2,
            };

            float neckLength = (float)Math.Sqrt(
                    Math.Pow((skeleton.Joints[second].Position.X - skeleton.Joints[first].Position.X), 2) +
                    Math.Pow((skeleton.Joints[second].Position.Y - skeleton.Joints[first].Position.Y), 2) +
                    Math.Pow((skeleton.Joints[second].Position.Z - skeleton.Joints[first].Position.Z), 2)
                );

            FrameData frame = new FrameData(TrackedJointsCount);

            foreach (JointType joint in _tracableSkeletonJoints)
            {
                frame.SkeletonJoints[(int)joint] = new SkeletonPoint()
                {
                    X = (skeleton.Joints[joint].Position.X - center.X) / neckLength,
                    Y = (skeleton.Joints[joint].Position.Y - center.Y) / neckLength,
                    Z = (skeleton.Joints[joint].Position.Z - center.Z) / neckLength,
                };
            }

            if (_frameBuffer.Count > _maximalBufferLength)
                _frameBuffer.Dequeue();

            _frameBuffer.Enqueue(frame);
            _lastFrame = frame;
        }
    }
}
