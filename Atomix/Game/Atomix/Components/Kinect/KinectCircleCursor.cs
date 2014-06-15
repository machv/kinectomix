﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Components.Kinect
{
    public class KinectCircleCursor : KinectCursor
    {
        private Texture2D _cicle;
        private AnimatedTexture _animatedCircle;
        private double _progress;
        private int _frame;
        private int _framesCount;
        private Vector2 _circlePosition;

        public double Progress
        {
            get { return _progress; }
            set
            {
                if (value >= 0 && value <= 1)
                    _progress = value;

                if (value > 1)
                    _progress = 1;

                if (value < 0)
                    _progress = 0;
            }
        }

        public KinectCircleCursor(Game game, KinectChooser chooser) : base(game, chooser)
        {
            _animatedCircle = new AnimatedTexture(Vector2.Zero, 0, 0.5f);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _framesCount = 18;
            _cicle = Game.Content.Load<Texture2D>("Images/HandCircle");
            _animatedCircle.Load(_cicle, _framesCount, _framesCount);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (HandPosition != Vector2.Zero)
            {
                _circlePosition = new Vector2(HandPosition.X - 20, HandPosition.Y - 20);
                _frame = (int)(_progress * _framesCount);
                if (_frame > _framesCount)
                    _frame = _framesCount - 1;

                System.Diagnostics.Debug.Print( string.Format("[U] Frame: {0}, Progress: {1}, Ratio: {2}", _frame, _progress, (_progress * _framesCount)));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (HandPosition != Vector2.Zero && _frame > 0)
            {
                _spriteBatch.Begin();
                _animatedCircle.DrawFrame(_spriteBatch, _frame, _circlePosition);
                System.Diagnostics.Debug.Print("Frame: " + _frame.ToString());
                _spriteBatch.End();
            }
        }
    }
}