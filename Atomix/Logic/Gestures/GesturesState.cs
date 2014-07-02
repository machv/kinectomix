using Mach.Kinect.Gestures;
using System.Collections.Generic;
using System.Linq;

namespace Kinectomix.Logic.Game
{
    public struct GesturesState
    {
        private IEnumerable<RecognizedGesture> _recognizedGestures;
        private KnownGestures _knownGestures;
        public GesturesState(IEnumerable<RecognizedGesture> recognizedGestures, KnownGestures knownGestures)
        {
            _recognizedGestures = recognizedGestures;
            _knownGestures = knownGestures;
        }

        public IEnumerable<RecognizedGesture> RecognizedGestures
        {
            get { return _recognizedGestures; }
        }

        public bool IsGestureRecognized(Gesture gesture)
        {
            return _recognizedGestures != null && _recognizedGestures.Where(g => g.Gesture == gesture).FirstOrDefault() != null;
        }

        public bool IsGestureRecognized(GestureType gesture)
        {
            return true; // _recognizedGestures != null && _recognizedGestures.Where(g => g.Gesture == _knownGestures[g].Instance).FirstOrDefault() != null;
        }
    }
}
