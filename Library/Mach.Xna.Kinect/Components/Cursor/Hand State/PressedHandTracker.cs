using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Kinect.HandState
{
    public class HoverHandTracker : IHandStateTracker
    {
        private bool _isHovered;
        private Vector2 _previousCursorPosition;
        private DateTime _hoverStart;
        private TimeSpan _minimalHoverDuration;
        private AnimatedTexture _animatedHand;

        public bool IsStateActive
        {
            get { return _isHovered; }
        }

        public HoverHandTracker()
        {
            _minimalHoverDuration = TimeSpan.FromSeconds(1);
            _animatedHand = new AnimatedTexture(Vector2.Zero, 0, 0.25f);
        }

        public void SetAnimationTexture(Texture2D texture, int frameCount, int framesPerSec)
        {
            _animatedHand.Load(texture, frameCount, framesPerSec);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset)
        {
            if (_previousCursorPosition != Vector2.Zero)
            {
                _animatedHand.DrawFrame(spriteBatch, _previousCursorPosition);
            }
        }

        public void ProcessDepthData(DepthImageFrame depthFrame) { }

        public void ProcessSkeletonData(SkeletonFrame frame) { }

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
    }
}
