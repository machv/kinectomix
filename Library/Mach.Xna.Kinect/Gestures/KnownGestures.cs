using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mach.Xna.Kinect.Gestures
{
    /// <summary>
    /// Collection of <see cref="KnownGesture"/>s.
    /// </summary>
    public class KnownGestures : Collection<KnownGesture>
    {
        /// <summary>
        /// Gets the <see cref="KnownGesture"/> of the specified type.
        /// </summary>
        /// <value>
        /// The <see cref="KnownGesture"/>.
        /// </value>
        /// <param name="type">The type of the gesture.</param>
        /// <returns><see cref="KnownGesture"/> of the specified type.</returns>
        public KnownGesture this[GestureType type]
        {
            get
            {
                return Items.Where(g => g.Type == type).FirstOrDefault();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownGestures"/> class from array of <see cref="KnownGesture"/>.
        /// </summary>
        /// <param name="gestures">The gestures.</param>
        public KnownGestures(KnownGesture[] gestures)
        {
            Array.ForEach(gestures, g => Items.Add(g));
        }
    }
}
