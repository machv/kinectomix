using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic.Gestures
{
    public class RecognizedGesture
    {
        public Gesture Gesture { get; set; }
        public Gesture Matching { get; set; }
        public double Distance { get; set; }
        public double Cost { get; set; }
    }
}
