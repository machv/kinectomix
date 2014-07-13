using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using System.Linq;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Recognizes registered gestures from skeletons.
    /// </summary>
    public class Recognizer : GestureProcessor
    {
        private double _lastFrameMatchThreshold = 2.5;
        private double _gestureMatchThreshold = 2;
        private List<Gesture> _gestures;
        private bool _isStarted;
        private List<RecognizedGesture> _recognizedGestures;
        private RecognizedGesture _recognizedGesture;
        private double _lastCost;

        /// <summary>
        /// Minimal length of the buffer containing recorded <see cref="GestureFrame"/>s.
        /// </summary>
        protected int _minimalBufferLength = 20;

        /// <summary>
        /// Gets or sets maximal accepted distance between currently recorded <see cref="GestureFrame"/> and last frame of known gestrure.
        /// </summary>
        /// <returns>Maximal accepted distance between currently recorded <see cref="GestureFrame"/> and last frame of known gestrure.</returns>
        public double LastFrameMatchThreshold
        {
            get { return _lastFrameMatchThreshold; }
            set { _lastFrameMatchThreshold = value; }
        }
        /// <summary>
        /// Gets or sets maximal accepted cost of the recorded candidate <see cref="Gesture"/> and known <see cref="Gesture"/>.
        /// </summary>
        /// <returns>Maximal accepted cost if the recorded candidate <see cref="Gesture"/> and known <see cref="Gesture"/>.</returns>
        public double GestureMatchThreshold
        {
            get { return _gestureMatchThreshold; }
            set { _gestureMatchThreshold = value; }
        }
        /// <summary>
        /// Gets or sets minimal length of the buffer containing recorded <see cref="GestureFrame"/>s.
        /// </summary>
        /// <returns>Minimal length of the buffer containing recorded <see cref="GestureFrame"/>s.</returns>
        public int MinimalBufferLength
        {
            get { return _minimalBufferLength; }
            set { if (value > 0) _minimalBufferLength = value; }
        }
        /// <summary>
        /// Gets all recognized <see cref="Gesture"/>s during last processing of Skeletons data.
        /// </summary>
        /// <returns>Recognized <see cref="Gesture"/>s.</returns>
        public List<RecognizedGesture> RecognizedGestures
        {
            get { return _recognizedGestures; }
        }
        /// <summary>
        /// Gets the last recognized <see cref="Gesture"/> during processing Skeletons data.
        /// </summary>
        /// <returns>Last recognized <see cref="Gesture"/>.</returns>
        public RecognizedGesture RecognizedGesture
        {
            get { return _recognizedGesture; }
        }
        /// <summary>
        /// Gets the cost of last evaluated candidate gesture.
        /// </summary>
        /// <returns>The cost of last evaluated candidate gesture.</returns>
        public double LastCost
        {
            get { return _lastCost; }
        }

        /// <summary>
        /// Initializes new instance of <see cref="Recognizer"/> class.
        /// </summary>
        public Recognizer()
        {
            _isStarted = false;
            _gestures = new List<Gesture>();
        }

        /// <summary>
        /// Starts recognizing of registered gestures.
        /// </summary>
        public void Start()
        {
            _isStarted = true;
        }

        /// <summary>
        /// Stops recognizing.
        /// </summary>
        public void Stop()
        {
            _isStarted = false;
        }

        /// <summary>
        /// Adds new <see cref="Gesture"/> to be recognized from the skeletons data.
        /// </summary>
        /// <param name="gesture">Gesture to be recognized.</param>
        public void AddGesture(Gesture gesture)
        {
            if (gesture.Sequence.Count < _minimalBufferLength)
                _minimalBufferLength = gesture.Sequence.Count;

            if (gesture.Sequence.Count > _maximalBufferLength)
                _maximalBufferLength = gesture.Sequence.Count;

            _gestures.Add(gesture);
        }

        /// <summary>
        /// Removes <see cref="Gesture"/> from recognizing.
        /// </summary>
        /// <param name="gesture">Gesture to remove.</param>
        public void RemoveGesture(Gesture gesture)
        {
            _gestures.Remove(gesture);
        }

        /// <summary>
        /// Extracts <see cref="Skeleton"/> points to gesture frame buffer and recognizes candidate gestures.
        /// </summary>
        /// <param name="skeleton"></param>
        public override void ProcessSkeleton(Skeleton skeleton)
        {
            _recognizedGesture = null;
            _recognizedGestures = new List<RecognizedGesture>();

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
                        {
                            _recognizedGesture = new RecognizedGesture(candidate, recordedGesture, cost, distance);
                            _recognizedGestures.Add(_recognizedGesture);
                        }
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
    }
}
