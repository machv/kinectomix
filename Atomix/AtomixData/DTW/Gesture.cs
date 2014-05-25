using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Kinectomix.Logic.DTW
{
    public class Gesture
    {
        public GestureTrackingDimension Dimension { get; set; }

        /// <summary>
        /// List of joints required to observe for this gesture.
        /// </summary>
        /// <returns></returns>
        public JointType[] TrackedJoints { get; set; }

        /// <summary>
        /// List of invariant positions ordered by TrackedJoints
        /// </summary>
        /// <returns></returns>
        public double[] GestureSequence { get; set; }
    }
}
