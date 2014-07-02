using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mach.Kinect
{
    /// <summary>
    /// Direction of detected swipe gesture.
    /// </summary>
    public enum SwipeDirection
    {
        /// <summary>
        /// Unknown swipe position.
        /// </summary>
        Unknown,
        /// <summary>
        /// Swipe up.
        /// </summary>
        Up,
        /// <summary>
        /// Swipe to right.
        /// </summary>
        Right,
        /// <summary>
        /// Swipe down.
        /// </summary>
        Down,
        /// <summary>
        /// Swipe to left.
        /// </summary>
        Left,
    }
}
