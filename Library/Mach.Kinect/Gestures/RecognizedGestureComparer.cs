﻿using System.Collections.Generic;

namespace Mach.Kinect.Gestures
{
    public class RecognizedGestureComparer : IEqualityComparer<RecognizedGesture>
    {
        public bool Equals(RecognizedGesture x, RecognizedGesture y)
        {
            return x.Gesture.Equals(y.Gesture);
        }

        public int GetHashCode(RecognizedGesture obj)
        {
            return obj.Gesture.GetHashCode();
        }
    }
}