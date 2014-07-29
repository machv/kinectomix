using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Base class for gesture <see cref="Recorder"/> and gesture <see cref="Recognizer"/>.
    /// </summary>
    public abstract class GestureProcessor
    {
        /// <summary>
        /// List of all available joints to track.
        /// </summary>
        protected static JointType[] _tracableSkeletonJoints;
        /// <summary>
        /// Buffer containing recorded frames from skeleton data.
        /// </summary>
        protected Queue<FrameData> _frameBuffer;
        /// <summary>
        /// Maximal length of the frame buffer.
        /// </summary>
        protected int _maximalBufferLength = 30;
        /// <summary>
        /// Last processed frame inserted into the frame buffer.
        /// </summary>
        protected FrameData _lastFrame;

        /// <summary>
        /// Gets or sets maximal length of the frame buffer.
        /// </summary>
        /// <returns>Maximal length of the frame buffer.</returns>
        public int MaximalBufferLength
        {
            get { return _maximalBufferLength; }
            set
            {
                if (value > 0)
                    _maximalBufferLength = value;
            }
        }
        /// <summary>
        /// Gets the current length of the frame buffer.
        /// </summary>
        /// <returns>The length of the frame buffer.</returns>
        public int FrameBufferLength
        {
            get { return _frameBuffer.Count; }
        }

        /// <summary>
        /// Static constructor that initializes <see cref="_tracableSkeletonJoints"/>.
        /// </summary>
        static GestureProcessor()
        {
            _tracableSkeletonJoints = (JointType[])Enum.GetValues(typeof(JointType));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GestureProcessor"/> class.
        /// </summary>
        public GestureProcessor()
        {
            _frameBuffer = new Queue<FrameData>(_maximalBufferLength);
        }

        /// <summary>
        /// Empties content of the frame buffer.
        /// </summary>
        public void Clear()
        {
            _frameBuffer.Clear();
        }

        /// <summary>
        /// Processes new <see cref="Skeleton"/> into the frame buffer.
        /// </summary>
        /// <param name="skeleton">Skeleton to transform into the frame buffer.</param>
        public virtual void ProcessSkeleton(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                if (_frameBuffer.Count > 0)
                    _frameBuffer.Dequeue();

                return;
            }

            // normalize of coordinates, 1 unit is length of shoulders (Head - ShoulderCenter) 
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

            FrameData frame = new FrameData(_tracableSkeletonJoints.Length);

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
