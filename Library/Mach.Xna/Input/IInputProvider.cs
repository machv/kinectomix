namespace Mach.Xna.Input
{
    /// <summary>
    /// Provides interface for reading state of the input method.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        /// Returns the current <see cref="IInputState"/>.
        /// </summary>
        /// <returns>Current <see cref="IInputState"/>.</returns>
        IInputState GetState();
    }
}
