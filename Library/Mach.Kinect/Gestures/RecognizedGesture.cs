namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Recognized gesture.
    /// </summary>
    public class RecognizedGesture
    {
        private Gesture _gesture;
        private Gesture _matching;
        private double _distance;
        private double _cost;

        /// <summary>
        /// Gets the original <see cref="Gesture"/> that was recognized.
        /// </summary>
        /// <returns>Original <see cref="Gesture"/>.</returns>
        public Gesture Gesture
        {
            get { return _gesture; }
        }
        /// <summary>
        /// Gets the matching recorded <see cref="Gesture"/>.
        /// </summary>
        /// <returns>Matching <see cref="Gesture"/>.</returns>
        public Gesture Matching
        {
            get { return _matching; }
        }
        /// <summary>
        /// Gets the distance between original and matching gesture.
        /// </summary>
        /// <returns>Distance between original and matching gesture.</returns>
        public double Distance
        {
            get { return _distance; }
        }
        /// <summary>
        /// Gets Distance cost between original and matching gesture.
        /// </summary>
        /// <returns>Distance cost between gestures.</returns>
        public double Cost
        {
            get { return _cost; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RecognizedGesture"/> class with details about recognized gesture.
        /// </summary>
        /// <param name="gesture">Original gesture.</param>
        /// <param name="matching">Matched gesture.</param>
        /// <param name="distance">Distance between original and matching gesture.</param>
        /// <param name="cost">Distance cost between original and matching gesture.</param>
        public RecognizedGesture(Gesture gesture, Gesture matching, double distance, double cost)
        {
            _gesture = gesture;
            _matching = matching;
            _distance = distance;
            _cost = cost;
        }
    }
}
