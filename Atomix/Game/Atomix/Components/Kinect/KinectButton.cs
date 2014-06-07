using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Atomix.Components.Kinect
{
    public class KinectButton : Button
    {
        private KinectCursor _cursor;
        public KinectButton(Game game, KinectCursor cursor)
            : base(game)
        {
            _cursor = cursor;
            _hoverDuration = TimeSpan.FromSeconds(1);
        }

        public KinectButton(Game game, KinectCursor cursor, string content)
            : this(game, cursor)
        {
            Content = content;
        }

        private bool _isHovering;
        private DateTime _hoverStart;
        private TimeSpan _hoverDuration;

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

            if (_isHovering == true && DateTime.Now - _hoverStart > _hoverDuration)
            {
                _isHovering = false;

                OnSelected();
            }
        }
    }
}
