using Microsoft.Xna.Framework;

namespace Kinectomix.Logic
{
    /// <summary>
    /// Direction of detected swipe gesture.
    /// </summary>
    public enum SwipeDirection
    {
        /// <summary>
        /// Unknown swipe position.
        /// </summary>
        Unknown,
        /// <summary>
        /// Swipe up.
        /// </summary>
        Up,
        /// <summary>
        /// Swipe to right.
        /// </summary>
        Right,
        /// <summary>
        /// Swipe down.
        /// </summary>
        Down,
        /// <summary>
        /// Swipe to left.
        /// </summary>
        Left,
    }

    /// <summary>
    /// Represents recognized swipe gesture.
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
        /// Initialize new instance with recognized values.
        /// </summary>
        public SwipeGesture(SwipeDirection direction, double distance)
        {
            _direction = direction;
            _distance = distance;
        }
    }

    /// <summary>
    /// Simple swipe gestures recognizer which can recognize 4 directions of swiping.
    /// </summary>
    public class SwipeGesturesRecognizer
    {
        private bool _verticalCandidate;
        private bool _horizontalCandidate;
        private Vector3 _startPosition;
        private Vector3 _previousPosition;
        private double _previousDiffX;
        private double _previousDiffY;
        private double _totalDistanceX;
        private double _totalDistanceY;
        private double _requiredDistance;

        private double _depthTolerance = 0.1d;
        private double _verticalTolerance = 0.03d;
        private double _horizontalTolerance = 0.03d;

        /// <summary>
        /// Gets or sets depth distance tolerance of points against the starting point in meters for swipe gesture.
        /// </summary>
        /// <returns>Depth tolerance in meters.</returns>
        public double DepthTolerance
        {
            get { return _depthTolerance; }
            set { _depthTolerance = value; }
        }
        /// <summary>
        /// Gets or sets vertical tolerance of points against the starting point in meters for swipe gesture.
        /// </summary>
        /// <returns>Vertical tolerane in meters.</returns>
        public double VerticalTolerance
        {
            get { return _verticalTolerance; }
            set { _verticalTolerance = value; }
        }
        /// <summary>
        /// Gets or sets horizontal tolerance of points against the starting point in meters for swipe gesture.
        /// </summary>
        /// <returns>Horizontal tolerane in meters.</returns>
        public double HorizontalTolerance
        {
            get { return _horizontalTolerance; }
            set { _horizontalTolerance = value; }
        }

        /// <summary>
        /// Starts new recognizing of swipe gesture based on startPosition and required length od swipe.
        /// </summary>
        /// <param name="startPosition">3D position of start swipe gesture in meters from .</param>
        /// <param name="requiredDistance">Required distance of swipe gestures in meters.</param>
        public void Start(Vector3 startPosition, double requiredDistance)
        {
            _startPosition = startPosition;
            _previousPosition = startPosition;
            _verticalCandidate = true;
            _horizontalCandidate = true;
            _previousDiffX = 0;
            _previousDiffY = 0;
            _totalDistanceX = 0;
            _totalDistanceY = 0;
            _requiredDistance = requiredDistance;
        }

        /// <summary>
        /// Adds current position to the recognizer to detect swipe gesture. If is false returned, processing can stop as recorded path of positions does not create swipe gesture.
        /// </summary>
        /// <param name="position">New position to process, in meters.</param>
        /// <param name="gesture">Recognized gesture.</param>
        /// <returns>True if this added position is valid candidate for swipe gesture. False if position does not belong to swipe gesture.</returns>
        public bool ProcessPosition(Vector3 position, out SwipeGesture gesture)
        {
            gesture = null;

            if (_verticalCandidate == false && _horizontalCandidate == false)
                return false;

            // real values are int meters
            if (position.Z < _startPosition.Z - _depthTolerance || position.Z > _startPosition.Z + _depthTolerance) // same depth from sensor
                return false; // Position is outside depth tolerance from starting point

            if (_verticalCandidate == true)
                _verticalCandidate = ProcessVerticalDirection(position);

            if (_verticalCandidate == true)
            {
                if (_totalDistanceY > _requiredDistance)
                {
                    gesture = new SwipeGesture(SwipeDirection.Up, _totalDistanceY);
                    _verticalCandidate = false;
                }

                if (_totalDistanceY < _requiredDistance * -1)
                {
                    gesture = new SwipeGesture(SwipeDirection.Down, _totalDistanceY * -1);
                    _verticalCandidate = false;
                }
            }

            if (_horizontalCandidate == true)
                _horizontalCandidate = ProcessHorizontalDirection(position);

            if (_horizontalCandidate == true)
            {
                if (_totalDistanceX > _requiredDistance)
                {
                    gesture = new SwipeGesture(SwipeDirection.Right, _totalDistanceX);
                    _horizontalCandidate = false;
                }

                if (_totalDistanceX < _requiredDistance * -1)
                {
                    gesture = new SwipeGesture(SwipeDirection.Left, _totalDistanceX * -1);
                    _horizontalCandidate = false;
                }
            }

            _previousPosition = position;

            return true;
        }

        private bool ProcessVerticalDirection(Vector3 position)
        {
            // Vertical gesture is within tolerance.
            if (position.X < _startPosition.X - _verticalTolerance || position.X > _startPosition.X + _verticalTolerance)
                return false; // Position is outside horizontal tolerance from starting point

            double diffY = position.Y - _previousPosition.Y;

            if (_previousDiffY != 0 && diffY < 0 && _previousDiffY > 0)
                return false; // Changing direction detected (eg to top, to bottom and back to top

            _totalDistanceY += diffY;
            _previousDiffY = diffY;

            return true;
        }

        private bool ProcessHorizontalDirection(Vector3 position)
        {
            // horizontal gesture is within tolerance
            if (position.Y < _startPosition.Y - _horizontalTolerance || position.Y > _startPosition.Y + _horizontalTolerance)
                return false; // Position is outside vertical tolerance from starting point

            double diffX = position.X - _previousPosition.X;

            if (_previousDiffX != 0 && diffX < 0 && _previousDiffX > 0)
                return false; // Changing direction detected (eg to left, to right and back to left

            _totalDistanceX += diffX;
            _previousDiffX = diffX;

            return true;
        }
    }
}
