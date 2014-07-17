using Microsoft.Xna.Framework.Input;

namespace Mach.Xna.Input
{
    /// <summary>
    /// Represents the state of a mouse input device, including mouse cursor position and left button state.
    /// </summary>
    public struct MouseInputState : IInputState
    {
        private MouseState _state;

        /// <summary>
        /// Gets the original <see cref="MouseState"/> structure.
        /// </summary>
        /// <value>
        /// The original <see cref="MouseState"/> structure.
        /// </value>
        public MouseState StateDetails
        {
            get { return _state; }
        }
        /// <summary>
        /// Gets the horizontal position of the mouse cursor.
        /// </summary>
        /// <returns>Horizontal position of the mouse cursor in relation to the upper-left corner of the game window.</returns>
        public int X
        {
            get { return _state.X; }
        }
        /// <summary>
        /// Gets the vertical position of the mouse cursor.
        /// </summary>
        /// <returns>Vertical position of the mouse cursor in relation to the upper-left corner of the game window.</returns>
        public int Y
        {
            get { return _state.Y; }
        }
        /// <summary>
        /// Gets is left mouse button is pressed.
        /// </summary>
        /// <returns>True if left mouse button is pressed.</returns>
        public bool IsStateActive
        {
            get { return _state.LeftButton == ButtonState.Pressed; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseInputState"/> structure.
        /// </summary>
        /// <param name="state"></param>
        public MouseInputState(MouseState state)
        {
            _state = state;
        }
    }
}
