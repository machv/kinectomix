using Atomix.Components;
using Atomix.Components.Common;
using Atomix.Components.Kinect;
using Kinectomix.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Atomix
{
    public class StartScreen : GameScreen
    {
        private SpriteFont _splashFont;
        private SpriteFont _normalFont;
        private SpriteBatch _spriteBatch;

        private KinectButton _startButton;
        private KinectButton _levelsButton;
        private KinectButton _quitButton;

        private KinectMessageBox _quitMessageBox;
        private KinectCursor _cursor;

        public StartScreen(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            _cursor = (ScreenManager.Game as AtomixGame).Cursor;

            _quitMessageBox = new KinectMessageBox(ScreenManager.Game, ScreenManager.InputProvider, _cursor);
            _quitMessageBox.Changed += _quitMessageBox_Changed;

            _startButton = new KinectButton(ScreenManager.Game, _cursor, "play game");
            _startButton.Selected += _startButton_Selected;

            _levelsButton = new KinectButton(ScreenManager.Game, _cursor, "levels");
            _levelsButton.Selected += _levelsButton_Selected;

            _quitButton = new KinectButton(ScreenManager.Game, _cursor, "quit game");
            _quitButton.Selected += _quitButton_Selected;

            Components.Add(_startButton);
            Components.Add(_levelsButton);
            Components.Add(_quitButton);

            Components.Add(_quitMessageBox);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _startButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _startButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 80);
            _levelsButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _startButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 20);
            _quitButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _quitButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 50 + _quitButton.Height);

            KeyboardState state = Keyboard.GetState();

            // Toggle full screen mode
            if (state.IsKeyDown(Keys.F11))
            {
                if (state.IsKeyDown(Keys.F11))
                {
                    AtomixGame game = (ScreenManager.Game as AtomixGame);
                    game.IsFullScreen = !game.IsFullScreen;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            string name = "Kinectomix";
            Vector2 size = _splashFont.MeasureString(name);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 220), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadContent()
        {
            _splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _quitMessageBox.Font = _normalFont;

            _startButton.Font = _normalFont;
            _startButton.InputProvider = ScreenManager.InputProvider;

            _levelsButton.Font = _normalFont;
            _levelsButton.InputProvider = ScreenManager.InputProvider;

            _quitButton.Font = _normalFont;
            _quitButton.InputProvider = ScreenManager.InputProvider;

            base.LoadContent();
        }

        private void _quitButton_Selected(object sender, EventArgs e)
        {
            if (_quitMessageBox.IsVisible)
                return;

            _quitMessageBox.Show("Do you really want to quit this game?", MessageBoxButtons.YesNo);
        }

        private void _levelsButton_Selected(object sender, EventArgs e)
        {
            if (_quitMessageBox.IsVisible)
                return;

            GameScreen screen = new LevelsScreen(_spriteBatch);

            ScreenManager.Add(screen);
            ScreenManager.Activate(screen);
        }

        private void _startButton_Selected(object sender, EventArgs e)
        {
            if (_quitMessageBox.IsVisible)
                return;

            LevelDefinition levelInfo = AtomixGame.State.GetCurrentLevel();

            Level currentLevel = LevelFactory.Load(string.Format("Content/Levels/{0}.atx", levelInfo.AssetName));

            LevelScreen gameScreen = new LevelScreen(currentLevel, _spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        private void _quitMessageBox_Changed(object sender, MessageBoxEventArgs e)
        {
            _quitMessageBox.Hide();

            if (e.Result == MessageBoxResult.Yes)
            {
                ScreenManager.Game.Exit();
            }
        }
    }
}
