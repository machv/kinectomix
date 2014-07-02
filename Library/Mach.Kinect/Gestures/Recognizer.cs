using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using System.Linq;

namespace Mach.Kinect.Gestures
{
    public class Recognizer : GestureProcessor
    {
        private double _lastFrameMatchThreshold = 2.5;
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

        protected int _minimalBufferLength = 20;
        public int MinimalBufferLength
        {
            get { return _minimalBufferLength; }
            set { if (value > 0) _minimalBufferLength = value; }
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
            if (gesture.Sequence.Count < _minimalBufferLength)
                _minimalBufferLength = gesture.Sequence.Count;

            if (gesture.Sequence.Count > _maximalBufferLength)
                _maximalBufferLength = gesture.Sequence.Count;

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
                var buffer = _frameBuffer.ToArray();
                IEnumerable<Tuple<double, Gesture>> candidates = GetCandidateGestures(_lastFrame);
                if (candidates != null)
                {
                    foreach (Tuple<double, Gesture> candidateInfo in candidates)
                    {
                        Gesture candidate = candidateInfo.Item2;

                        Gesture recordedGesture = Gesture.FromFrameData(buffer, candidate.TrackedJoints, candidate.Dimension);
                        double distance = DynamicTimeWarping.CalculateDistance(candidate, recordedGesture);
                        double cost = distance / buffer.Length;

                        _lastCost = cost;

                        if (cost < _gestureMatchThreshold)
                            _recognizedGesture = new RecognizedGesture() { Gesture = candidate, Cost = cost, Distance = distance, Matching = recordedGesture };
                        else
                        {
                            System.Diagnostics.Debug.Print(string.Format("False positive | {0} | {1} | {2}", candidate.Name, candidateInfo.Item1, cost));
                        }
                    }
                }
            }

            base.ProcessSkeleton(skeleton);
        }

        private IEnumerable<Tuple<double, Gesture>> GetCandidateGestures(FrameData lastFrame)
        {
            foreach (Gesture gesture in _gestures)
            {
                GestureFrame frame = GestureFrame.FromFrameData(lastFrame, gesture.TrackedJoints, gesture.Dimension);

                double frameDistance = DynamicTimeWarping.AccumulatedEuclidianDistance(frame, gesture.Sequence[gesture.Sequence.Count - 1], gesture.Dimension);
                if (frameDistance < _lastFrameMatchThreshold)
                {
                    yield return Tuple.Create(frameDistance, gesture);
                }
                else
                {
                    System.Diagnostics.Debug.Print(string.Format("Frame distance | {1} | {0}", frameDistance, gesture.Name));
                }
            }
        }

        public void RemoveGesture(Gesture gesture)
        {
            _gestures.Remove(gesture);
        }
    }
}
