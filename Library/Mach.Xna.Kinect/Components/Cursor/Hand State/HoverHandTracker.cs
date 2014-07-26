using System;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Detects hand hover.
    /// </summary>
    public class HoverHandTracker : IHandStateTracker
    {
        private bool _isHovered;
        private Vector2 _previousCursorPosition;
        private DateTime _hoverStart;
        private TimeSpan _minimalHoverDuration;
        private AnimatedTexture _animatedHand;

        /// <summary>
        /// Gets if tracked hand is performing hover.
        /// </summary>
        /// <returns>True if tracked hand is performing hover.</returns>
        public bool IsStateActive
        {
            get { return _isHovered; }
        }
        /// <summary>
        /// Gets or sets minimal duration of hand hover.
        /// </summary>
        /// <returns>Minimal duration of hand hover.</returns>
        public TimeSpan MinimalHoverDuration
        {
            get { return _minimalHoverDuration; }
            set { _minimalHoverDuration = value; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HoverHandTracker"/> class.
        /// </summary>
        public HoverHandTracker()
        {
            _minimalHoverDuration = TimeSpan.FromSeconds(1);
            _animatedHand = new AnimatedTexture(Vector2.Zero, 0, 0.25f);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HoverHandTracker" /> class.
        /// </summary>
        /// <param name="minimalHoverDuration">Minimal duration of the hover.</param>
        public HoverHandTracker(TimeSpan minimalHoverDuration)
        {
            _minimalHoverDuration = minimalHoverDuration;
            _animatedHand = new AnimatedTexture(Vector2.Zero, 0, 0.25f);
        }

        /// <summary>
        /// Sets hover animation.
        /// </summary>
        /// <param name="texture">Texture containing animation frames.</param>
        /// <param name="frameCount">How many frames is in texture.</param>
        /// <param name="framesPerSec">Speed of the animation.</param>
        public void SetAnimationTexture(Texture2D texture, int frameCount, int framesPerSec)
        {
            _animatedHand.Load(texture, frameCount, framesPerSec);
        }

        /// <summary>
        /// Draws hover animation.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <param name="spriteBatch"><see cref="SpriteBatch"/> used for drawing.</param>
        /// <param name="font">Font for drawing debug information.</param>
        /// <param name="scale">Scale for displayed information.</param>
        /// <param name="renderOffset">Offset for displayed information.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset)
        {
            if (_previousCursorPosition != Vector2.Zero)
            {
                _animatedHand.DrawFrame(spriteBatch, _previousCursorPosition);
            }
        }

        /// <summary>
        /// Updates status of the hover.
        /// </summary>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="cursorPosition">Current position of the cursor.</param>
        public void Update(bool leftHanded, Vector2 cursorPosition)
        {
            if (cursorPosition == Vector2.Zero)
            {
                _animatedHand.Stop();
                _hoverStart = DateTime.MinValue;
                _isHovered = false;
                _previousCursorPosition = Vector2.Zero;
                return;
            }

            if (Vector2.Distance(_previousCursorPosition, cursorPosition) < 10)
            {
                if (_hoverStart == DateTime.MinValue)
                {
                    _hoverStart = DateTime.Now;
                    _animatedHand.Reset();
                    _animatedHand.Play();
                }

                TimeSpan duration = DateTime.Now - _hoverStart;

                _animatedHand.UpdateFrame(duration.TotalSeconds);

                if (duration > _minimalHoverDuration)
                {
                    _isHovered = true;
                    _animatedHand.Pause();
                }
            }
            else
            {
                _animatedHand.Stop();
                _hoverStart = DateTime.MinValue;
                _isHovered = false;
            }

            _previousCursorPosition = cursorPosition;
        }

        /// <summary>
        /// Not used in this implementation.
        /// </summary>
        /// <param name="depthFrame">Depth frame to process.</param>
        public void ProcessDepthData(DepthImageFrame depthFrame) { }

        /// <summary>
        /// Not used in this implementation.
        /// </summary>
        /// <param name="frame">Skeleton frame to process.</param>
        public void ProcessSkeletonData(SkeletonFrame frame) { }
    }
}
