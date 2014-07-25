using Mach.Xna.Kinect.Components;

namespace Mach.Xna.Input
{
    /// <summary>
    /// Represents a state recorded by the Kinect sensor.
    /// </summary>
    public struct KinectInputState : IInputState
    {
        private int _x;
        private int _y;
        private bool _isStateActive;

        /// <summary>
        /// Gets the horizontal position of the Kinect cursor.
        /// </summary>
        /// <returns>Horizontal position of the Kinect cursor.</returns>
        public int X
        {
            get { return _x; }
        }
        /// <summary>
        /// Gets the vertical position of the Kinect cursor.
        /// </summary>
        /// <returns>Vertical position of the Kinect cursor.</returns>
        public int Y
        {
            get { return _y; }
        }
        /// <summary>
        /// Gets status of the observed hand state.
        /// </summary>
        /// <returns>True if observed hand state is active.</returns>
        public bool IsStateActive
        {
            get { return _isStateActive; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectInputState"/> structure.
        /// </summary>
        /// <param name="cursor">The Kinect cursor handling input from user.</param>
        public KinectInputState(KinectCursor cursor)
            : this()
        {
            _x = (int)cursor.Position.X;
            _y = (int)cursor.Position.Y;
            _isStateActive = cursor.IsHandStateActive;
        }
    }
}
