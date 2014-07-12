using System.Collections.Generic;
using Microsoft.Kinect;
using System;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Records new gesture.
    /// </summary>
    public class Recorder : GestureProcessor
    {
        private IEnumerable<JointType> _trackedJoints;
        private TrackingDimension _dimension;
        private bool _isStarted;

        /// <summary>
        /// Starts recording frames for a new gesture into the frame buffer until <see cref="GetRecordedGesture"/> method is used for retrieve recorded gesture.
        /// If called again before previous gesture is retrieved via <see cref="GetRecordedGesture"/>, recorded frames will be flushed.
        /// </summary>
        /// <param name="trackedJoints">Collection of significant joints that will be tracked for the gesture.</param>
        /// <param name="dimension">Tracking dimension for points of tracked joints.</param>
        public void Start(IEnumerable<JointType> trackedJoints, TrackingDimension dimension)
        {
            Clear();

            _maximalBufferLength = int.MaxValue;
            _trackedJoints = trackedJoints;
            _dimension = dimension;
            _isStarted = true;
        }

        /// <summary>
        /// Returns new gesture created from the recorded frames since <see cref="Start(IEnumerable{JointType}, TrackingDimension)"/> was called.
        /// </summary>
        /// <returns>Recorded gesture.</returns>
        public Gesture GetRecordedGesture()
        {
            if (_isStarted == false)
                throw new InvalidOperationException("Recording is not started.");

            _isStarted = false;

            return Gesture.FromFrameData(_frameBuffer, _trackedJoints, _dimension);
        }

        /// <summary>
        /// Extracts <see cref="Skeleton"/> points to gesture frame buffer.
        /// </summary>
        /// <param name="skeleton">Skeleton retrieved from the Kinect sensor.</param>
        public override void ProcessSkeleton(Skeleton skeleton)
        {
            if (!_isStarted)
            {
                return;
            }

            base.ProcessSkeleton(skeleton);
        }
    }
}
