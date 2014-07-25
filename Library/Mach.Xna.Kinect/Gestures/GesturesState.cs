using Mach.Kinect.Gestures;
using System.Collections.Generic;
using System.Linq;

namespace Mach.Xna.Kinect.Gestures
{
    /// <summary>
    /// Contains state of the recognized gestures.
    /// </summary>
    public struct GesturesState
    {
        private IEnumerable<RecognizedGesture> _recognizedGestures;
        private KnownGestures _knownGestures;

        /// <summary>
        /// Gets the all recognized gestures.
        /// </summary>
        /// <value>
        /// The all recognized gestures.
        /// </value>
        public IEnumerable<RecognizedGesture> RecognizedGestures
        {
            get { return _recognizedGestures; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GesturesState"/> structure.
        /// </summary>
        /// <param name="recognizedGestures">The recognized gestures.</param>
        /// <param name="knownGestures">The known gestures.</param>
        public GesturesState(IEnumerable<RecognizedGesture> recognizedGestures, KnownGestures knownGestures)
        {
            _recognizedGestures = recognizedGestures;
            _knownGestures = knownGestures;
        }

        /// <summary>
        /// Determines whether is specified gesture recognized.
        /// </summary>
        /// <param name="gesture">The gesture.</param>
        /// <returns><c>true</c> if specified gesture was recognized.</returns>
        public bool IsGestureRecognized(Gesture gesture)
        {
            return _recognizedGestures != null && _recognizedGestures.Where(g => g.Gesture == gesture).FirstOrDefault() != null;
        }

        /// <summary>
        /// Determines whether is specified gesture recognized.
        /// </summary>
        /// <param name="gesture">The type of the gesture.</param>
        /// <returns><c>true</c> if specified gesture was recognized.</returns>
        public bool IsGestureRecognized(GestureType gesture)
        {
            KnownGestures known = _knownGestures;
            return _recognizedGestures != null && _recognizedGestures.Where(g => g.Gesture == known[gesture].Instance).FirstOrDefault() != null;
        }
    }
}
