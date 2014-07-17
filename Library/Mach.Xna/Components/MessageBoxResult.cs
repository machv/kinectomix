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
        /// <summary>
        /// This dialog box return value is unknown (usually sent from a custom button which does not have defined Tag value).
        /// </summary>
        Unknown,
        /// <summary>
        /// The dialog box return value is Custom 1 (can be sent from a user-defined button).
        /// </summary>
        Custom1,
        /// <summary>
        /// The dialog box return value is Custom 2 (can be sent from a user-defined button).
        /// </summary>
        Custom2,
        /// <summary>
        /// The dialog box return value is Custom 3 (can be sent from a user-defined button).
        /// </summary>
        Custom3,
        /// <summary>
        /// The dialog box return value is Custom 4 (can be sent from a user-defined button).
        /// </summary>
        Custom4,
        /// <summary>
        /// The dialog box return value is Custom 5 (can be sent from a user-defined button).
        /// </summary>
        Custom5,
    }
}
