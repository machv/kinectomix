using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Kinectomix.Logic.DTW
{
    public class GestureRecognizer : GestureProcessor
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

        public GestureRecognizer()
        {
            _gestures = new List<Gesture>();
        }

        public void AddGesture(Gesture gesture)
        {
            //TODO takhle nastavovat maximal
            if (gesture.GestureSequence.Count > _minimalBufferLength)
                _minimalBufferLength = gesture.GestureSequence.Count;

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
                    double distance = DynamicTimeWarping.CalculateDtw(gesture, Gesture.FromFrameData(_frameBuffer, gesture.TrackedJoints, gesture.Dimension));
                    double cost = distance / _frameBuffer.Count;

                    _lastCost = cost;
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
