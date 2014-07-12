namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Specifies what dimension for points is used when processing gestures.
    /// </summary>
    public enum TrackingDimension
    {
        /// <summary>
        /// Use only x and y axes.
        /// </summary>
        Two = 2,
        /// <summary>
        /// Use all x, y and z axes. Will use also depth information.
        /// </summary>
        Three = 3,
    }
}
