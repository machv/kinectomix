using System.Collections.Generic;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Compares two <see cref="RecognizedGesture"/>s if same gesture is recognized.
    /// </summary>
    public class RecognizedGestureComparer : IEqualityComparer<RecognizedGesture>
    {
        /// <summary>
        /// Determines whether the specified <see cref="RecognizedGesture"/>s contains same <see cref="Gesture"/>.
        /// </summary>
        /// <param name="gesture1">First gesture to compare.</param>
        /// <param name="gesture2">Second gesture to compare.</param>
        /// <returns>True if both <see cref="RecognizedGesture"/> recognized same gesture.</returns>
        public bool Equals(RecognizedGesture gesture1, RecognizedGesture gesture2)
        {
            return gesture1.Gesture.Equals(gesture2.Gesture);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="RecognizedGesture"/>.
        /// </summary>
        /// <param name="gesture">The <see cref="RecognizedGesture"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified <see cref="RecognizedGesture"/>.</returns>
        public int GetHashCode(RecognizedGesture gesture)
        {
            return gesture.Gesture.GetHashCode();
        }
    }
}
