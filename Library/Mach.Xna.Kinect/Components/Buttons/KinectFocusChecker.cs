using Microsoft.Xna.Framework;
using System;

namespace Mach.Xna.Kinect.Components
{
    public class KinectFocusChecker
    {
        private KinectCursor _cursor;
        private KinectCircleCursor _circleCursor;
        private bool _isHovering;
        private DateTime _hoverStart;
        private TimeSpan _minimalHoverDuration;

        /// <summary>
        /// Gets or sets minimal required duration of cursor's hover over button to accept it as "click". Default hover duration is 1 second.
        /// </summary>
        /// <returns>Duration required to accept.</returns>
        public TimeSpan MinimalHoverDuration
        {
            get { return _minimalHoverDuration; }
            set { _minimalHoverDuration = value; }
        }

        private KinectFocusChecker()
        {
            _minimalHoverDuration = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Initializes new instance of <see cref="KinectFocusChecker"/> with <see cref="KinectCircleCursor"/>. Default hover time is 1 second.
        /// </summary>
        /// <param name="cursor">Cursor from which track position.</param>
        public KinectFocusChecker(KinectCircleCursor cursor)
            : this()
        {
            if (cursor == null)
                throw new ArgumentNullException("cursor");

            _circleCursor = cursor;
        }

        /// <summary>
        /// Initializes new instance of <see cref="KinectFocusChecker"/> with <see cref="KinectCursor"/>. Default hover time is 1 second.
        /// </summary>
        /// <param name="cursor">Cursor from which track position.</param>
        public KinectFocusChecker(KinectCursor cursor)
            : this()
        {
            if (cursor == null)
                throw new ArgumentNullException("cursor");

            if (cursor is KinectCircleCursor)
                _circleCursor = cursor as KinectCircleCursor;
            else
                _cursor = cursor;
        }

        public bool ProcessCursorFocus(Rectangle boundingRectangle, out bool isSelected)
        {
            isSelected = false;

            Vector2 handPosition = _circleCursor != null ? _circleCursor.Position : _cursor.Position;
            bool isOver = boundingRectangle.Contains((int)handPosition.X, (int)handPosition.Y);

            if (isOver == true && _isHovering == false)
            {
                _hoverStart = DateTime.Now;
                _isHovering = true;

                if (_circleCursor != null)
                {
                    _circleCursor.Progress = 0;
                }
            }

            if (isOver == false && _isHovering == true)
            {
                // Was hovering -> reset
                _isHovering = false;

                if (_circleCursor != null)
                {
                    _circleCursor.Progress = 0;
                }
            }

            if (_isHovering == true)
            {
                TimeSpan elapsed = DateTime.Now - _hoverStart;

                if (_circleCursor != null)
                {
                    _circleCursor.Progress = elapsed.TotalMilliseconds / _minimalHoverDuration.TotalMilliseconds;
                }

                if (elapsed > _minimalHoverDuration)
                {
                    _isHovering = false;

                    if (_circleCursor != null)
                    {
                        _circleCursor.Progress = 0;
                    }

                    isSelected = true;
                }
            }

            return isOver;
        }
    }
}
