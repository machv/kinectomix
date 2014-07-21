using System;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Button that interacts with Kinect sensor.
    /// </summary>
    public interface IKinectAwareButton
    {
        /// <summary>
        /// Gets or sets minimal required duration of cursor's hover over button to accept it as "click". Default hover duration is 1 second.
        /// </summary>
        /// <returns>Duration required to accept.</returns>
        TimeSpan MinimalHoverDuration { get; set; }
    }
}
