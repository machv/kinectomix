using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using System.Linq;

namespace Kinectomix.Logic.Gestures
{
    public class Recognizer : GestureProcessor
    {
        private double _lastFrameMatchThreshold = 2;
        private double _gestureMatchThreshold = 2;

        public double LastFrameMatchThreshold
        {
            get { return _lastFrameMatchThreshold; }
            set { _lastFrameMatchThreshold = value; }
        }

        public double GestureMatchThreshold
        {
            get { return _gestureMatchThreshold; }
            set { _gestureMatchThreshold = value; }
        }

        private List<Gesture> _gestures;
        private bool _isStarted = false;

        public void Start()
        {
            _isStarted = true;
        }

        public void Stop()
        {
            _isStarted = false;
        }

        public Recognizer()
        {
            _gestures = new List<Gesture>();
        }

        public void AddGesture(Gesture gesture)
        {
            //TODO takhle nastavovat maximal
            if (gesture.Sequence.Count > _minimalBufferLength)
                _minimalBufferLength = gesture.Sequence.Count;

            _gestures.Add(gesture);
        }

        private RecognizedGesture _recognizedGesture;
        public RecognizedGesture RecognizedGesture
        {
            get { return _recognizedGesture; }
            set { _recognizedGesture = value; }
        }

        private double _lastCost;
        public double LastCost { get { return _lastCost; } }
        public override void ProcessSkeleton(Skeleton skeleton)
        {
            _recognizedGesture = null;

            if (!_isStarted)
                return;

            if (_frameBuffer.Count >= _minimalBufferLength)
            {
                Gesture candidate = GetClosestCandidate(_lastFrame);
                if (candidate != null)
                {
                    Gesture recordedGesture = Gesture.FromFrameData(_frameBuffer, candidate.TrackedJoints, candidate.Dimension);
                    double distance = DynamicTimeWarping.CalculateDistance(candidate, recordedGesture);
                    double cost = distance / _frameBuffer.Count;

                    _lastCost = cost;

                    if (cost < _gestureMatchThreshold)
                        _recognizedGesture = new RecognizedGesture() { Gesture = candidate, Cost = cost, Distance = distance, Matching = recordedGesture };
                }
            }

            base.ProcessSkeleton(skeleton);
        }

        private Gesture GetClosestCandidate(FrameData lastFrame)
        {
            double minimalFrameDistance = double.MaxValue;
            Gesture candidate = null;

            foreach (Gesture gesture in _gestures)
            {
                GestureFrame frame = GestureFrame.FromFrameData(lastFrame, gesture.TrackedJoints, gesture.Dimension);

                double frameDistance = DynamicTimeWarping.AccumulatedEuclidianDistance(frame, gesture.Sequence[gesture.Sequence.Count - 1], gesture.Dimension);
                if (frameDistance < _lastFrameMatchThreshold && frameDistance < minimalFrameDistance)
                {
                    candidate = gesture;
                    minimalFrameDistance = frameDistance;
                }
                else
                    System.Diagnostics.Debug.Print(string.Format("Frame distance: {0} from {1}", frameDistance, gesture.Name));
            }

            return candidate;
        }

        public void RemoveGesture(Gesture gesture)
        {
            _gestures.Remove(gesture);
        }
    }
}
