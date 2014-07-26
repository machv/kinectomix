using Microsoft.Kinect.Toolkit.Interaction;

namespace Mach.Xna.Kinect
{
    //http://blogs.msdn.com/b/k4wdev/archive/2013/05/01/using-kinect-interactionstream-outside-of-wpf.aspx
    /// <summary>
    /// Simple Kinect Interaction client implementation that accepts input all the time.
    /// </summary>
    public class KinectInteractionClient : IInteractionClient
    {
        /// <summary>
        /// Gets the interaction information at location.
        /// </summary>
        /// <param name="skeletonTrackingId">The skeleton tracking identifier.</param>
        /// <param name="handType">Type of the hand.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>Information about this user interaction.</returns>
        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            var interactionInfo = new InteractionInfo
            {
                IsPressTarget = true,
                IsGripTarget = true
            };

            return interactionInfo;
        }
    }
}
