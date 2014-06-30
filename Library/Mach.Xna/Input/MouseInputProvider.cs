using Microsoft.Xna.Framework.Input;

namespace Mach.Xna.Input
{
    /// <summary>
    /// Input provider for mouse state.
    /// </summary>
    public class MouseInputProvider : IInputProvider
    {
        /// <summary>
        /// Gets the current state of the mouse, including mouse position and left button state.
        /// </summary>
        /// <returns>Current state of the mouse.</returns>
        public IInputState GetState()
        {
            MouseState mouseState = Mouse.GetState();

            return new MouseInputState(mouseState);
        }
    }
}
