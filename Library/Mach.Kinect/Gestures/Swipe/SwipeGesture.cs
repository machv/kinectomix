namespace Mach.Kinect.Gestures
{
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
        /// Initialize new instance with recognized values.
        /// </summary>
        public SwipeGesture(SwipeDirection direction, double distance)
        {
            _direction = direction;
            _distance = distance;
        }
    }
}
