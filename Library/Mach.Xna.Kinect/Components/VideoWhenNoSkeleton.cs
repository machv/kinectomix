using Mach.Kinect;
using Microsoft.Xna.Framework;
using System;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Shows video stream and rendered skeleton when no fully tracked skeleton is available.
    /// </summary>
    public class VideoWhenNoSkeleton : DrawableGameComponent
    {
        private readonly TimeSpan AfterMatchTimeout = TimeSpan.FromSeconds(2);

        private KinectManager _kinectManager;
        private VideoStreamComponent _video;
        private SkeletonRenderer _skeleton;
        private bool _showVideo;
        private DateTime _matchDate;
        private bool _wasPreviouslySkeletonPresent;
        private float _transparency;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoWhenNoSkeleton"/> class.
        /// </summary>
        /// <param name="game">The game containing this component.</param>
        /// <param name="kinectManager">The kinect manager.</param>
        /// <param name="video">The video stream component.</param>
        /// <param name="skeleton">The skeleton rendering component.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public VideoWhenNoSkeleton(Game game, KinectManager kinectManager, VideoStreamComponent video, SkeletonRenderer skeleton)
            : base(game)
        {
            if (kinectManager == null || video == null || skeleton == null)
                throw new ArgumentNullException();

            _kinectManager = kinectManager;
            _video = video;
            _skeleton = skeleton;
        }

        /// <summary>
        /// Initializes all components.
        /// </summary>
        public override void Initialize()
        {
            _video.Initialize();
            _skeleton.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// Checks if any skeleton is available and if not shows video preview.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (_kinectManager.Sensor != null && _kinectManager.Skeletons != null)
            {
                if (_kinectManager.Skeletons.TrackedSkeleton == null)
                {
                    _showVideo = true;
                    _transparency = 1;
                }
                else
                {
                    // We have tracked skeleton -> check if timeout exceeded
                    if (_wasPreviouslySkeletonPresent == false)
                    {
                        _matchDate = DateTime.Now;
                    }

                    TimeSpan difference = DateTime.Now - _matchDate;
                    if (difference < AfterMatchTimeout)
                    {
                        // Lower transparency
                        _transparency = 1 - (float)(difference.TotalMilliseconds) / (float)(AfterMatchTimeout.TotalMilliseconds);
                    }
                    else
                    {
                        // After timeout exceeds hide video completely
                        _showVideo = false;
                    }
                }

                if (_showVideo)
                {
                    _video.Transparency = _transparency;
                    _video.Update(gameTime);
                    _skeleton.Update(gameTime);
                    _skeleton.Transparency = _transparency;
                }

                _wasPreviouslySkeletonPresent = _kinectManager.Skeletons.TrackedSkeleton != null;
            }
            else
            {
                _showVideo = false;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws video preview when no skeleton is tracked.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_showVideo)
            {
                _video.Draw(gameTime);
                _skeleton.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
