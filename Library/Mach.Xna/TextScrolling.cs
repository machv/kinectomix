namespace Mach.Xna
{
    /// <summary>
    /// Defines type of scrolling of the text.
    /// </summary>
    public enum TextScrolling
    {
        /// <summary>
        /// No scrolling.
        /// </summary>
        None,
        /// <summary>
        /// Infinite loop of scrolling from left to right and at the right side of the will restart and starts again from left.
        /// </summary>
        Loop,
        /// <summary>
        /// Infinite loop of sliding from left to right and back to left.
        /// </summary>
        Slide,
    }
}
