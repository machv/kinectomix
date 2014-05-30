﻿using System.Collections.Generic;
using Microsoft.Kinect;

namespace Kinectomix.Logic.Gestures
{
    public class Recognizer : GestureProcessor
    {
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

        private double _lastCost;
        public override void ProcessSkeleton(Skeleton skeleton)
        {
            if (!_isStarted)
                return;

            if (_frameBuffer.Count >= _minimalBufferLength)
            {
                foreach (Gesture gesture in _gestures)
                {
                    Gesture candidate = Gesture.FromFrameData(_frameBuffer, gesture.TrackedJoints, gesture.Dimension);
                    double frameDistance = DynamicTimeWarping.AccumulatedEuclidianDistance(candidate.Sequence[candidate.Sequence.Count - 1], gesture.Sequence[gesture.Sequence.Count - 1], gesture.Dimension);
                    if (frameDistance < 2)
                    {
                        double distance = DynamicTimeWarping.CalculateDtw(gesture, Gesture.FromFrameData(_frameBuffer, gesture.TrackedJoints, gesture.Dimension));
                        double cost = distance / _frameBuffer.Count;

                        _lastCost = cost;
                    }
                    else
                        System.Diagnostics.Debug.Print(string.Format("Frame distance: {0}", frameDistance));
                }
            }

            base.ProcessSkeleton(skeleton);
        }

        public void Recognize(List<double[]> sequence)
        {
            foreach (Gesture gesture in _gestures)
            {
            }
        }

        public double GetLastCost()
        {
            return _lastCost;
        }
    }
}
