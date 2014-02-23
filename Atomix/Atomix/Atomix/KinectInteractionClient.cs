using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    class KinectInteractionClient : IInteractionClient
    {

        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            InteractionInfo ii = new InteractionInfo();

            return ii;
        }
    }
}
