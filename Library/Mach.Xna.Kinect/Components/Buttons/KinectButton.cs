using System;
using Microsoft.Xna.Framework;
using Mach.Xna.Components;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Extended implementation of <see cref="Button"/> class that interacts with Kinect using <see cref="KinectCursor"/> or <see cref="KinectCircleCursor"/>.
    /// </summary>
    public class KinectButton : Button, IKinectAwareButton
    {
        private KinectFocusChecker _focusChecker;
        private TimeSpan _minimalHoverDuration;

        /// <summary>
        /// Gets or sets minimal required duration of cursor's hover over button to accept it as "click". Default hover duration is 1 second.
        /// </summary>
        /// <returns>Duration required to accept.</returns>
        public TimeSpan MinimalHoverDuration
        {
            get { return _minimalHoverDuration; }
            set
            {
                _minimalHoverDuration = value;

                if (_focusChecker != null)
                {
                    _focusChecker.MinimalHoverDuration = value;
                }
            }
        }

        private KinectButton(Game game)
            : base(game)
        {
            _minimalHoverDuration = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Initializes new instance of <see cref="KinectButton"/> with <see cref="KinectCursor"/>. Default hover time is 1 second.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="cursor">Cursor from which track position.</param>
        public KinectButton(Game game, KinectCursor cursor)
            : this(game)
        {
            if (cursor == null)
                throw new ArgumentNullException("cursor");

            _focusChecker = new KinectFocusChecker(cursor);
        }

        /// <summary>
        /// Initializes new instance of <see cref="KinectButton"/> with cursor and text caption.
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
        /// Process checks in <see cref="Button"/> Update and adds additional logic for hover via <see cref="KinectCursor"/>.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool isSelected;

            _isFocused = _focusChecker.ProcessCursorFocus(_boundingRectangle, out isSelected);

            if (isSelected)
            {
                OnSelected();
            }
        }
    }
}
