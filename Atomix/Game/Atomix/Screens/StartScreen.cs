using Mach.Kinectomix.Logic;
using Mach.Kinectomix.Resources;
using Mach.Xna.Components;
using Mach.Xna.Input.Extensions;
using Mach.Xna.Kinect.Components;
using Mach.Xna.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mach.Kinectomix.Screens
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
        private KinectCircleCursor _cursor;

        public StartScreen(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            _cursor = (ScreenManager.Game as KinectomixGame).Cursor;

            _quitMessageBox = new KinectMessageBox(ScreenManager.Game, ScreenManager.InputProvider, _cursor);
            _quitMessageBox.Changed += _quitMessageBox_Changed;

            _startButton = new KinectButton(ScreenManager.Game, _cursor, StartScreenResources.PlayGame);
            _startButton.Selected += Start_Selected;

            _levelsButton = new KinectButton(ScreenManager.Game, _cursor, StartScreenResources.Levels);
            _levelsButton.Selected += Levels_Selected;

            _quitButton = new KinectButton(ScreenManager.Game, _cursor, StartScreenResources.QuitGame);
            _quitButton.Selected += Quit_Selected;

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
                    KinectomixGame game = (ScreenManager.Game as KinectomixGame);
                    game.IsFullScreen = !game.IsFullScreen;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            string name = StartScreenResources.GameName;
            Vector2 size = _splashFont.MeasureString(name);

            _spriteBatch.Begin();
            _spriteBatch.DrawStringWithShadow(_splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 220), Color.DarkRed);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            _splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Title");
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

        public override void Activated()
        {
            (ScreenManager.Game as KinectomixGame).VisualKinectManager.ShowConnectKinectPrompt = true;

            base.Activated();
        }

        public override void Deactivated()
        {
            (ScreenManager.Game as KinectomixGame).VisualKinectManager.ShowConnectKinectPrompt = false;

            base.Deactivated();
        }

        private void Levels_Selected(object sender, EventArgs e)
        {
            GameScreen screen = new LevelsScreen(_spriteBatch);

            ScreenManager.Add(screen);
            ScreenManager.Activate(screen);
        }

        private void Start_Selected(object sender, EventArgs e)
        {
            Level level = KinectomixGame.State.GetCurrentLevel();
            LevelScreen gameScreen = new LevelScreen(level, _spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        private void Quit_Selected(object sender, EventArgs e)
        {
            _levelsButton.IsEnabled = false;
            _quitButton.IsEnabled = false;
            _startButton.IsEnabled = false;

            _quitMessageBox.Show(StartScreenResources.QuitConfirmation, MessageBoxButtons.YesNo);
        }

        private void _quitMessageBox_Changed(object sender, MessageBoxEventArgs e)
        {
            _quitMessageBox.Hide();

            _levelsButton.IsEnabled = true;
            _quitButton.IsEnabled = true;
            _startButton.IsEnabled = true;

            if (e.Result == MessageBoxResult.Yes)
            {
                ScreenManager.Game.Exit();
            }
        }
    }
}
