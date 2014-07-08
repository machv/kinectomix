namespace Mach.Xna.Components
{
    /// <summary>
    /// Specifies identifiers to indicate the return value of a message box.
    /// </summary>
    public enum MessageBoxResult
    {
        /// <summary>
        /// The dialog box return value is Cancel (usually sent from a button labeled Cancel).
        /// </summary>
        Cancel,
        /// <summary>
        /// The dialog box return value is No (usually sent from a button labeled No).
        /// </summary>
        No,
        /// <summary>
        /// The dialog box return value is Yes (usually sent from a button labeled Yes).
        /// </summary>
        Yes,
        /// <summary>
        /// The dialog box return value is OK (usually sent from a button labeled OK).
        /// </summary>
        OK,
    }
}
