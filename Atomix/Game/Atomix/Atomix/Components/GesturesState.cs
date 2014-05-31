using Atomix.Components;
using Kinectomix.Logic.Gestures;

namespace Atomix
{
    public struct GesturesState
    {
        private RecognizedGesture _recognizedGesture; 
        internal GesturesState(RecognizedGesture recognizedGesture)
        {
            _recognizedGesture = recognizedGesture;
        }

        public RecognizedGesture RecognizedGesture
        {
            get { return _recognizedGesture; }
        }

        public bool IsGestureRecognized(Gesture gesture)
        {
            return gesture == _recognizedGesture.Gesture;
        }
    }
}