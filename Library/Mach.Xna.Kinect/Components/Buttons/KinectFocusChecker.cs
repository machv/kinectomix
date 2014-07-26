using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Provides logic for handling focus of the cursor within bounding rectangle.
    /// </summary>
    public class KinectFocusChecker
    {
        private KinectCursor _cursor;
        private KinectCircleCursor _circleCursor;
        private bool _isHovering;
        private DateTime _hoverStart;
        private TimeSpan _minimalHoverDuration;
        private bool _isKinectTracking;
        private bool _trackMouseIfKinectIsNotTracking;

        /// <summary>
        /// Gets or sets minimal required duration of cursor's hover over button to accept it as "click". Default hover duration is 1 second.
        /// </summary>
        /// <returns>Duration required to accept.</returns>
        public TimeSpan MinimalHoverDuration
        {
            get { return _minimalHoverDuration; }
            set { _minimalHoverDuration = value; }
        }
        /// <summary>
        /// Gets a value indicating whether is kinect tracking hand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is kinect tracking hand; otherwise, <c>false</c>.
        /// </value>
        public bool IsKinectTracking
        {
            get { return _isKinectTracking; }
        }
        /// <summary>
        /// Gets or sets a whether mouse will be tracked when Kinect is not tracking cursor.
        /// </summary>
        /// <value>
        /// <c>true</c> if mouse will be tracked when Kinect is not tracking cursor; otherwise, <c>false</c>.
        /// </value>
        public bool TrackMouseIfKinectIsNotTracking
        {
            get { return _trackMouseIfKinectIsNotTracking; }
            set { _trackMouseIfKinectIsNotTracking = value; }
        }

        private KinectFocusChecker()
        {
            _minimalHoverDuration = TimeSpan.FromSeconds(1);
            _trackMouseIfKinectIsNotTracking = true;
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

        /// <summary>
        /// Processes the cursor focus within selected bounding rectangle.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle in which focus will be checked.</param>
        /// <param name="isSelected">if set to <c>true</c> required time for selection has been met.</param>
        /// <returns><c>true</c> if cursor is inside the bounding rectangle; otherwise <c>false</c>.</returns>
        public bool ProcessCursorFocus(Rectangle boundingRectangle, out bool isSelected)
        {
            isSelected = false;

            KinectCursor cursor = _circleCursor != null ? _circleCursor : _cursor;

            Vector2 handPosition = _circleCursor != null ? _circleCursor.Position : _cursor.Position;

            if (cursor != null && !cursor.IsHandTracked)
            {
                if (_trackMouseIfKinectIsNotTracking)
                {
                    MouseState state = Mouse.GetState();
                    handPosition = new Vector2(state.X, state.Y);
                }

                _isKinectTracking = false;
            }
            else
            {
                _isKinectTracking = true;
            }

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
