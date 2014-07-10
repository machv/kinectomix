using Mach.Kinect;
using Mach.Kinectomix.Logic;
using Mach.Xna.Components;
using Mach.Xna.Kinect.Components;
using Mach.Xna.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Mach.Kinectomix.Screens
{
    public class LevelsScreen : GameScreen
    {
        public const int ButtonStep = 20;

        private SpriteBatch _spriteBatch;
        private List<KinectButton> _buttons = new List<KinectButton>();
        private SpriteFont _normalFont;
        private SpriteFont _splashFont;
        private SwipeGesturesRecognizer swipe;
        private KinectCursor _cursor;
        private KinectButton _backButton;

        public LevelsScreen(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            _cursor = (ScreenManager.Game as KinectomixGame).Cursor;

            _backButton = new KinectButton(ScreenManager.Game, _cursor, "go back");
            _backButton.Selected += Back_Selected;

            Components.Add(_backButton);

            foreach (Level level in KinectomixGame.State.Levels)
            {
                KinectButton button = new KinectButton(ScreenManager.Game, _cursor);
                button.Selected += button_Selected;
                button.Tag = level;
                button.Content = level.Name;
                button.InputProvider = ScreenManager.InputProvider;
                button.Width = 320;

                _buttons.Add(button);

                Components.Add(button);
            }

            swipe = new SwipeGesturesRecognizer();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _backButton.Font = _normalFont;
            _backButton.InputProvider = ScreenManager.InputProvider;

            foreach (Button button in _buttons)
                button.Font = _normalFont;

            base.LoadContent();
        }

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private int xTranslation;
        private int xTranslationBuffer;
        private int xStep = 10;
        public override void Update(GameTime gameTime)
        {
            _backButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width + _backButton.BorderThickness  - _backButton.Width, -_backButton.BorderThickness);

            int xPos = 20;
            int xDiff = 30;
            int yPos = 20;

            Vector2 position = new Vector2(xPos, 170);

            foreach (Button b in _buttons)
            {
                b.Position = position;

                position.X += xDiff;
                position.Y += b.Height + yPos;

                if (position.Y + b.Height + yPos > ScreenManager.GraphicsDevice.Viewport.Bounds.Height)
                {
                    xPos += xDiff + b.Width;
                    position.Y = 170;
                    position.X = xPos;
                }
            }

            foreach (Button b in _buttons)
            {
                var pos = b.Position;
                pos.X += xTranslation;
                b.Position = pos;
            }

            int scrollStep = ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 4 * 1;

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left) == true && _previousKeyboardState.IsKeyDown(Keys.Left) == false)
                xTranslationBuffer = scrollStep;

            if (keyboardState.IsKeyDown(Keys.Right) == true && _previousKeyboardState.IsKeyDown(Keys.Right) == false)
                xTranslationBuffer = -scrollStep;

            _previousKeyboardState = keyboardState;

            MouseState mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
                xTranslationBuffer = scrollStep;

            if (mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
                xTranslationBuffer = -scrollStep;

            _previousMouseState = mouseState;

            // Scrolling for gestures is larger
            scrollStep = ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 4 * 3;

            if (_cursor.IsHandTracked && xTranslationBuffer == 0)
            {
                SwipeGesture recognized;
                if (swipe.ProcessPosition(_cursor.HandRealPositionPoint, out recognized))
                {
                    if (recognized != null)
                    {
                        var direction = recognized.Direction;
                        switch (direction)
                        {
                            case SwipeDirection.Left:
                                xTranslationBuffer = -scrollStep;
                                break;
                            case SwipeDirection.Right:
                                xTranslationBuffer = scrollStep;
                                break;
                        }
                    }
                }
                else
                {
                    swipe.Start(_cursor.HandRealPositionPoint, 0.08);
                }
            }

            if (xTranslationBuffer != 0)
            {
                if (xTranslation >= 0)
                {
                    xTranslation = 0;
                    xTranslationBuffer = 0;
                }

                if (xTranslationBuffer > 0)
                {
                    xTranslationBuffer -= xStep;
                    xTranslation += xStep;
                    if (xTranslationBuffer < 0)
                        xTranslationBuffer = 0;
                }
                else
                {
                    xTranslationBuffer += xStep;
                    xTranslation -= xStep;
                    if (xTranslationBuffer > 0)
                        xTranslationBuffer = 0;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(_splashFont, "Game Levels", new Vector2(20, 30), Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Back_Selected(object sender, EventArgs e)
        {
            ScreenManager.Activate(ScreenManager.PreviousScreen);
        }

        private void button_Selected(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Level level = button.Tag as Level;

            LevelScreen gameScreen = new LevelScreen(level, _spriteBatch);

            KinectomixGame.State.SetLevelToCurrent(level);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }
    }
}
