using Mach.Kinect.Gestures;
using Mach.Kinectomix.Logic;
using Mach.Xna;
using Mach.Xna.Components;
using Mach.Xna.Input.Extensions;
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
        private SpriteFont _titleFont;
        private SwipeRecognizer swipe;
        private KinectCursor _cursor;
        private Texture2D _backgroundTexture;
        private SpriteButton _backButton;

        public LevelsScreen(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            _cursor = (ScreenManager.Game as KinectomixGame).Cursor;

            _backButton = new KinectSpriteButton(ScreenManager.Game, _cursor);
            _backButton.Selected += Back_Selected;
            Components.Add(_backButton);

            for (int i = 0; i < KinectomixGame.State.Levels.Length; i++)
            {
                Level level = KinectomixGame.State.Levels[i];
                LevelHighscore highscore = KinectomixGame.State.Highscore.GetLevelHighscore(i);

                bool isEnabled = true;
                if (i > 0 && highscore == null) // Skips first level as first will be available always
                {
                    LevelHighscore previousHighscore = KinectomixGame.State.Highscore.GetLevelHighscore(i - 1);
                    if (previousHighscore == null) // If previous level is not finished, we do not allow to play this one
                    {
                        isEnabled = false;
                    }
                }

                KinectButton button = new KinectButton(ScreenManager.Game, _cursor);
                button.Selected += button_Selected;
                button.Tag = level;
                button.Content = level.Name;
                button.InputProvider = ScreenManager.InputProvider;
                button.Width = 360 - button.Padding * 2;
                button.Height = 60 - button.Padding * 2;
                button.BorderThickness = 0;
                button.Background = Color.Transparent;
                button.Foreground = Color.White;
                button.ActiveBackground = Color.Black;
                button.DisabledBackground = Color.Transparent;
                button.DisabledForeground = Color.Gray;
                button.IsEnabled = isEnabled;
                button.TextAlignment = TextAlignment.Left;
                button.TextScrolling = TextScrolling.Loop;

                _buttons.Add(button);
                Components.Add(button);
            }

            swipe = new SwipeRecognizer();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _backgroundTexture = ScreenManager.Content.Load<Texture2D>("Backgrounds/Levels");
            _titleFont = ScreenManager.Content.Load<SpriteFont>("Fonts/LevelName");
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _backButton.Texture = ScreenManager.Content.Load<Texture2D>("Buttons/BackNormal");
            _backButton.Focused = ScreenManager.Content.Load<Texture2D>("Buttons/BackFocused");
            _backButton.Width = 60;
            _backButton.Height = 60;
            _backButton.InputProvider = ScreenManager.InputProvider;

            foreach (Button button in _buttons)
                button.Font = _normalFont;

            base.LoadContent();
        }

        private void FreezeButtons()
        {
            if (_buttons == null)
                return;

            foreach (ButtonBase b in _buttons)
                b.Freeze();
        }
        private void UnfreezeButtons()
        {
            if (_buttons == null)
                return;

            foreach (ButtonBase b in _buttons)
                b.Unfreeze();
        }

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private int xTranslation;
        private int xTranslationBuffer;
        private int xStep = 10;
        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            // Cheat for allowing all levels
            if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.A) && state.IsKeyDown(Keys.L))
            {
                foreach (Button button in _buttons)
                {
                    button.IsEnabled = true;
                }
            }

            _backButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width - _backButton.Width - 30, 30);

            int topOffet = 150;
            int xPos = 50;
            int xDiff = 60;
            int yPos = 60;
            int rightMargin = 30;

            Vector2 position = new Vector2(xPos, topOffet);

            foreach (Button b in _buttons)
            {
                b.Position = position;
                position.Y += b.ActualHeight + yPos;

                if (position.Y + b.Height > ScreenManager.GraphicsDevice.Viewport.Bounds.Height)
                {
                    xPos += xDiff + b.ActualWidth;
                    position.Y = topOffet;
                    position.X = xPos;
                }
            }

            bool anyButtonOverflows = false;
            int rightOverflow = 0;
            foreach (Button b in _buttons)
            {
                var pos = b.Position;
                pos.X += xTranslation;
                b.Position = pos;

                if (b.Position.X + b.Width > ScreenManager.GraphicsDevice.Viewport.Bounds.Width)
                {
                    anyButtonOverflows = true;
                    rightOverflow = Math.Max(rightOverflow, (int)b.Position.X + b.Width - ScreenManager.GraphicsDevice.Viewport.Bounds.Width);
                }
            }

            int scrollStep = ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 4 * 1;
            scrollStep = 2 * (360 + 60);

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left) == true && _previousKeyboardState.IsKeyDown(Keys.Left) == false)
                if (xTranslation < 0)
                    xTranslationBuffer = scrollStep;

            if (keyboardState.IsKeyDown(Keys.Right) == true && _previousKeyboardState.IsKeyDown(Keys.Right) == false)
                if (anyButtonOverflows)
                    xTranslationBuffer = -scrollStep;

            _previousKeyboardState = keyboardState;

            MouseState mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
                if (xTranslation < 0)
                    xTranslationBuffer = scrollStep;

            if (mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
                if (anyButtonOverflows)
                    xTranslationBuffer = -scrollStep;

            _previousMouseState = mouseState;

            // Scrolling for gestures is larger
            scrollStep = ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 4 * 3;
            scrollStep = 360;

            if (_cursor.IsHandTracked && xTranslationBuffer == 0)
            {
                SwipeGesture recognized;
                if (swipe.ProcessPosition(_cursor.HandPosition, out recognized))
                {
                    if (recognized != null)
                    {
                        var direction = recognized.Direction;
                        switch (direction)
                        {
                            case SwipeDirection.Left:
                                if (anyButtonOverflows)
                                    xTranslationBuffer = -scrollStep;
                                break;
                            case SwipeDirection.Right:
                                if (xTranslation < 0)
                                    xTranslationBuffer = scrollStep;
                                break;
                        }
                    }
                }
                else
                {
                    swipe.Start(_cursor.HandPosition, 0.08);
                }
            }

            if (xTranslationBuffer > 0 && xTranslation < 0) // If we scroll to right...
                if (xTranslation + xTranslationBuffer > 0)  // ...and buffer contains more than starting alignment...
                    xTranslationBuffer = xTranslation * -1; // ...we crop buffer it to correct alignment.

            if (xTranslationBuffer < 0) // If we scroll to left...
                if (xTranslationBuffer * -1 > rightOverflow + rightMargin) // ...and buffer contains more than maximal right overflow...
                    xTranslationBuffer = rightOverflow * -1 - rightMargin; // we crop buffer to right overflow + margin.


            if (xTranslationBuffer != 0) // Start animation
            {
                FreezeButtons();

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

                if (xTranslationBuffer == 0)
                {
                    // Animation has finished
                    UnfreezeButtons();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the screen.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, ScreenManager.Game.GraphicsDevice.Viewport.Bounds.Width, ScreenManager.Game.GraphicsDevice.Viewport.Bounds.Height), Color.White);
            _spriteBatch.DrawStringWithShadow(_titleFont, Resources.LevelsScreenResources.Title, new Vector2(55, 40), KinectomixGame.BrickColor);

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
