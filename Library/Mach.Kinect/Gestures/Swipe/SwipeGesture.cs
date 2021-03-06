﻿namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Detected swipe gesture.
    /// </summary>
    public class SwipeGesture
    {
        private SwipeDirection _direction;
        private double _distance;

        /// <summary>
        /// Gets which swipe direction was detected.
        /// </summary>
        /// <returns>Detected swipe direction.</returns>
        public SwipeDirection Direction
        {
            get { return _direction; }
        }
        /// <summary>
        /// Gets distance of this detected gesture.
        /// </summary>
        /// <returns>Detected swipe gesture distance from starting point.</returns>
        public double Distance
        {
            get { return _distance; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwipeGesture"/> class.
        /// </summary>
        /// <param name="direction">The direction of swipe gesture.</param>
        /// <param name="distance">The distance in meters of currently detected swipe gesture.</param>
        public SwipeGesture(SwipeDirection direction, double distance)
        {
            _direction = direction;
            _distance = distance;
        }
    }
}
