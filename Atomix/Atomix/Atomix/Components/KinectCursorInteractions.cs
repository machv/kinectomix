using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;

namespace Atomix.Components
{
    public class KinectCursorInteractions : KinectCursor
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_KinectChooser.Sensor != null && _KinectChooser.SkeletonData != null)
                _KinectChooser.Interactions.ProcessSkeleton(_KinectChooser.SkeletonData, _KinectChooser.Sensor.AccelerometerGetCurrentReading(), _KinectChooser.SkeletonTimestamp);

            using (DepthImageFrame depthFrame = _KinectChooser.Sensor.DepthStream.OpenNextFrame(0))
            {
                if (depthFrame != null)
                {
                    // Create array for pixel data and copy it from the image frame
                    DepthImagePixel[] depthPixels = new DepthImagePixel[depthFrame.PixelDataLength];
                    lastDepthFrame.CopyDepthImagePixelDataTo(depthPixels);

                    _KinectChooser.Interactions.ProcessDepth(depthPixels, lastDepthFrame.Timestamp);
                }
            }

            using (InteractionFrame frame = _KinectChooser.Interactions.OpenNextFrame(0))
            {
                if (frame != null)
                {
                    UserInfo[] info = new UserInfo[6];
                    frame.CopyInteractionDataTo(info);

                    var usr = info.Where(i => i.SkeletonTrackingId > 0).FirstOrDefault();
                    if (usr != null)
                    {
                        foreach (var interaction in usr.HandPointers)
                        {
                            if (interaction.HandType == InteractionHandType.Right)
                            {
                                //_textToRender = string.Format("Interaction: [{0}x{1}]", interaction.X, interaction.Y, _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.X, _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.Y);

                                if (interaction.HandEventType == InteractionHandEventType.Grip)
                                {
                                    IsHandClosed = true;
                                }
                                else if (interaction.HandEventType == InteractionHandEventType.GripRelease)
                                {
                                    IsHandClosed = false;
                                }

                                //cursorPositionInteraction = new Vector2();
                                //cursorPositionInteraction.X = (int)(interaction.X * GraphicsDevice.Viewport.Bounds.Width);
                                //cursorPositionInteraction.Y = (int)(interaction.Y * GraphicsDevice.Viewport.Bounds.Height);
                            }
                        }
                    }
                }
            }
        }
    }
}
