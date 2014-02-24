using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    class KinectInteractionClient : IInteractionClient
    {
        int _width;
        int _height;

        public KinectInteractionClient(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            var interactionInfo = new InteractionInfo
            {
                IsPressTarget = false,
                IsGripTarget = false
            };

            // Map coordinates from [0.0,1.0] coordinates to UI-relative coordinates
            double xUI = x * _width;
            double yUI = y * _height;

            interactionInfo.IsPressTarget = true;

            // If UI framework uses strings as button IDs, use string hash code as ID
            interactionInfo.PressTargetControlId = 0;

            // Designate center of button to be the press attraction point
            //// TODO: Create your own logic to assign press attraction points if center
            //// TODO: is not always the desired attraction point.
            //interactionInfo.PressAttractionPointX = ((uiElement.Left + uiElement.Right) / 2.0) / _width;
            //interactionInfo.PressAttractionPointY = ((uiElement.Top + uiElement.Bottom) / 2.0) / _height;

            return interactionInfo;
        }
    }
}
