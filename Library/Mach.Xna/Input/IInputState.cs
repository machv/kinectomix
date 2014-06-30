namespace Mach.Xna.Input
{
    /// <summary>
    /// Represents a state recorded by a input device.
    /// </summary>
    public interface IInputState
    {
        /// <summary>
        /// Gets the horizontal position from the input device.
        /// </summary>
        /// <returns>Horizontal position from the input device.</returns>
        int X { get; }
        /// <summary>
        /// Gets the vertical position from the input device.
        /// </summary>
        /// <returns>Vertical position from the input device.</returns>
        int Y { get; }
        /// <summary>
        /// Gets status of the input device.
        /// </summary>
        /// <returns>True if observed state is active.</returns>
        bool IsStateActive { get; }
    }
}
