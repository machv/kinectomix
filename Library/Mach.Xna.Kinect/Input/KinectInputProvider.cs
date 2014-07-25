using Mach.Xna.Kinect.Components;

namespace Mach.Xna.Input
{
    /// <summary>
    /// Reads input state from the Kinect sensor.
    /// </summary>
    public class KinectInputProvider : IInputProvider
    {
        /// <summary>
        /// The Kinect cursor.
        /// </summary>
        protected KinectCursor _cursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectInputProvider"/> class.
        /// </summary>
        /// <param name="cursor">The Kinect cursor handling input from user.</param>
        public KinectInputProvider(KinectCursor cursor)
        {
            _cursor = cursor;
        }

        /// <summary>
        /// Returns the current <see cref="KinectInputState"/> from Kinect sensor.
        /// </summary>
        /// <returns>Current <see cref="KinectInputState"/>.</returns>
        public IInputState GetState()
        {
            return new KinectInputState(_cursor);
        }
    }
}
