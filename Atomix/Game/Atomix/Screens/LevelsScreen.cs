using Atomix.Components;
using Kinectomix.Logic;
using Kinectomix.Xna.Components;
using Kinectomix.Xna.Components.Kinect;
using Kinectomix.Xna.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class LevelsScreen : GameScreen
    {
        public const int ButtonStep = 20;

        private SpriteBatch _spriteBatch;
        private List<KinectButton> _buttons = new List<KinectButton>();
        private SpriteFont _normalFont;
        private SpriteFont _splashFont;
        private SwipeGesturesRecognizer swipe;
        private KinectCursor cursor;

        public LevelsScreen(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            cursor = (ScreenManager.Game as AtomixGame).Cursor;

            foreach (Level level in AtomixGame.State.Levels)
            {
                KinectButton button = new KinectButton(ScreenManager.Game, cursor);
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

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(_splashFont, "Game Levels", new Vector2(20, 30), Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private int xTranslation;
        private int xTranslationBuffer;
        private int xStep = 10;
        public override void Update(GameTime gameTime)
        {
            int xPos = 20;
            int xDiff = 30;
            int yPos = 20;

            Vector2 position = new Vector2(xPos, 170);

            //int xTranslation = -150;

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

            int scrollStep = ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 4 * 3;

            if (cursor.IsHandTracked && xTranslationBuffer == 0)
            {
                SwipeGesture recognized;
                if (swipe.ProcessPosition(cursor.HandRealPosition, out recognized))
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
                    swipe.Start(cursor.HandRealPosition, 0.08);
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

        public override void LoadContent()
        {
            _splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            foreach (Button button in _buttons)
                button.Font = _normalFont;

            base.LoadContent();
        }

        private void button_Selected(object sender, EventArgs e)
        {
            return;

            Button button = sender as Button;
            Level level = button.Tag as Level;

            LevelScreen gameScreen = new LevelScreen(level, _spriteBatch);

            AtomixGame.State.SetLevelToCurrent(level);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }
    }
}
