﻿using Microsoft.Kinect.Toolkit.Interaction;

namespace Mach.Xna.Kinect
{
    //http://blogs.msdn.com/b/k4wdev/archive/2013/05/01/using-kinect-interactionstream-outside-of-wpf.aspx
    /// <summary>
    /// Simple Kinect Interaction client implementation that accepts input all the time.
    /// </summary>
    public class KinectInteractionClient : IInteractionClient
    {
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