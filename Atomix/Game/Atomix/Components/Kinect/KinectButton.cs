using System;
using Microsoft.Xna.Framework;

namespace Atomix.Components.Kinect
{
    /// <summary>
    /// Extended implementation of <see cref="Button"/> class that interacts with Kinect using <see cref="KinectCursor"/>.
    /// </summary>
    public class KinectButton : Button
    {
        private KinectCursor _cursor;
        private bool _isHovering;
        private DateTime _hoverStart;
        private TimeSpan _minimalHoverDuration;

        /// <summary>
        /// Gets or sets minimal required duration of cursor's hover over button to accept it as "click".
        /// </summary>
        /// <returns>Duration required to accept.</returns>
        public TimeSpan MinimalHoverDuration
        {
            get { return _minimalHoverDuration; }
            set { _minimalHoverDuration = value; }
        }

        /// <summary>
        /// Constructs new instance of <see cref="KinectButton"/> with cursor.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="cursor">Cursor from which track position.</param>
        public KinectButton(Game game, KinectCursor cursor)
            : base(game)
        {
            _cursor = cursor;
            _minimalHoverDuration = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Constructs new instance of <see cref="KinectButton"/> with cursor and text caption.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="cursor">Cursor from which track position.</param>
        /// <param name="content">Caption text for the button.</param>
        public KinectButton(Game game, KinectCursor cursor, string content)
            : this(game, cursor)
        {
            Content = content;
        }

        /// <summary>
        /// Overrides update from base <see cref="Button"/> class and checks for hover.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 handPosition = _cursor.HandPosition;

            bool isOver = _boundingRectangle.Contains((int)handPosition.X, (int)handPosition.Y);

            if (isOver == true && _isHovering == false)
            {
                _hoverStart = DateTime.Now;
                _isHovering = true;
            }

            if (isOver == false)
            {
                _isHovering = false;
            }

            if (_isHovering == true && DateTime.Now - _hoverStart > _minimalHoverDuration)
            {
                _isHovering = false;

                OnSelected();
            }
        }
    }
}
