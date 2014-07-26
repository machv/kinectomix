using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Linq;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Tracks if active hand is closed.
    /// </summary>
    public class InteractionsClosedHandTracker : IHandStateTracker
    {
        private InteractionStream _interationsStream;
        private int _skeletonArrayLength;
        private VisualKinectManager _chooser;
        private bool _isHandClosed;

        /// <summary>
        /// Gets if tracked hand is closed.
        /// </summary>
        /// <returns>True if tracked hand is closed.</returns>
        public bool IsStateActive
        {
            get { return _isHandClosed; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InteractionsClosedHandTracker"/> class.
        /// </summary>
        /// <param name="kinectManager">Kinect manager handling Kinect.</param>
        public InteractionsClosedHandTracker(VisualKinectManager kinectManager)
        {
            KinectInteractionClient ic = new KinectInteractionClient();
            _interationsStream = new InteractionStream(kinectManager.Sensor, ic);
            _chooser = kinectManager;
        }

        /// <summary>
        /// Processes depth frame.
        /// </summary>
        /// <param name="depthFrame">Depth frame to process.</param>
        public void ProcessDepthData(DepthImageFrame depthFrame)
        {
            if (depthFrame != null)
            {
                DepthImagePixel[] depthPixels = new DepthImagePixel[depthFrame.PixelDataLength];
                depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                _interationsStream.ProcessDepth(depthPixels, depthFrame.Timestamp);
            }
        }

        /// <summary>
        /// Processes skeleton frame.
        /// </summary>
        /// <param name="skeletonFrame">Skeleton frame to process.</param>
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

        /// <summary>
        /// Updates status of the tracked closed/open hand.
        /// </summary>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="cursorPosition">Current position of the cursor.</param>
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

        /// <summary>
        /// Not used in this implementation.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <param name="spriteBatch"><see cref="SpriteBatch"/> used for drawing.</param>
        /// <param name="font">Font for drawing debug information.</param>
        /// <param name="scale">Scale for displayed information.</param>
        /// <param name="renderOffset">Offset for displayed information.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset) { }
    }
}
