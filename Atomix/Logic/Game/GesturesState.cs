using Kinectomix.Logic.Gestures;

namespace Kinectomix.Logic.Game
{
    public struct GesturesState
    {
        private RecognizedGesture _recognizedGesture;
        private KnownGestures _knownGestures;
        public GesturesState(RecognizedGesture recognizedGesture, KnownGestures knownGestures)
        {
            _recognizedGesture = recognizedGesture;
            _knownGestures = knownGestures;
        }

        public RecognizedGesture RecognizedGesture
        {
            get { return _recognizedGesture; }
        }

        public bool IsGestureRecognized(Gesture gesture)
        {
            return _recognizedGesture != null && _recognizedGesture.Gesture == gesture;
        }

        public bool IsGestureRecognized(GestureType gesture)
        {
            return _recognizedGesture != null && _recognizedGesture.Gesture == _knownGestures[gesture].Instance;
        }
    }
}
