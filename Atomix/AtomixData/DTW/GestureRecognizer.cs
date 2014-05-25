using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic.DTW
{
    public class GestureRecognizer
    {
        private List<Gesture> _gestures;

        public GestureRecognizer()
        {
            _gestures = new List<Gesture>();
        }

        public void AddGesture(Gesture gesture)
        {
            _gestures.Add(gesture);
        }

        public void Recognize(List<double[]> sequence)
        {
            foreach (Gesture gesture in _gestures)
            {
            }
        }
    }
}
