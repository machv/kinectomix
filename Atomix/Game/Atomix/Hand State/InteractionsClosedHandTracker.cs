using System;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Linq;

namespace Atomix
{
    public class InteractionsClosedHandTracker : IHandStateTracker
    {
        private InteractionStream _interationsStream;
        private int _skeletonArrayLength;
        private VisualKinectManager _chooser;
        private bool _isHandClosed;

        public bool IsStateActive
        {
            get { return _isHandClosed; }
        }

        public InteractionsClosedHandTracker(VisualKinectManager chooser)
        {
            KinectInteractionClient ic = new KinectInteractionClient();
            _interationsStream = new InteractionStream(chooser.Sensor, ic);
            _chooser = chooser;
        }


        public void ProcessDepthData(DepthImageFrame depthFrame)
        {
            if (depthFrame != null)
            {
                DepthImagePixel[] depthPixels = new DepthImagePixel[depthFrame.PixelDataLength];
                depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                _interationsStream.ProcessDepth(depthPixels, depthFrame.Timestamp);
            }
        }

        public void ProcessSkeletonData(SkeletonFrame skeletonFrame)
        {
            if (skeletonFrame != null)
            {
                Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletonData);
                _skeletonArrayLength = skeletonFrame.SkeletonArrayLength;

                _interationsStream.ProcessSkeleton(skeletonData, _chooser.Sensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);
            }
        }

        public void Update(bool leftHanded, Vector2 cursorPosition)
        {
            using (InteractionFrame frame = _interationsStream.OpenNextFrame(0))
            {
                if (frame != null)
                {
                    UserInfo[] info = new UserInfo[_skeletonArrayLength];
                    frame.CopyInteractionDataTo(info);

                    var usr = info.Where(i => i.SkeletonTrackingId > 0).FirstOrDefault();
                    if (usr != null)
                    {
                        foreach (var interaction in usr.HandPointers)
                        {
                            if (interaction.HandType == InteractionHandType.Right)
                            {
                                if (interaction.HandEventType == InteractionHandEventType.Grip)
                                {
                                    _isHandClosed = true;
                                }
                                else if (interaction.HandEventType == InteractionHandEventType.GripRelease)
                                {
                                    _isHandClosed = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset) { }
    }
}
