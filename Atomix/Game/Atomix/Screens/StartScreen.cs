using Atomix.Components;
using Kinectomix.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class StartScreen : GameScreen
    {
        SpriteFont splashFont;
        SpriteFont normalFont;
        SpriteBatch spriteBatch;

        Button _startButton;
        Button _levelsButton;
        Button _quitButton;

        private MessageBox _quitMessageBox;

        public StartScreen(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            _quitMessageBox = new MessageBox(ScreenManager.Game);

            _startButton = new Button(ScreenManager.Game, "play game");
            _startButton.Selected += _startButton_Selected;

            _levelsButton = new Button(ScreenManager.Game, "levels");
            _levelsButton.Selected += _levelsButton_Selected;

            _quitButton = new Button(ScreenManager.Game, "quit game");
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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            string name = "Kinectomix";
            Vector2 size = splashFont.MeasureString(name);

            spriteBatch.Begin();
            spriteBatch.DrawString(splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 220), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadContent()
        {
            splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _quitMessageBox.Font = normalFont;

            _startButton.Font = normalFont;
            _startButton.InputProvider = ScreenManager.InputProvider;

            _levelsButton.Font = normalFont;
            _levelsButton.InputProvider = ScreenManager.InputProvider;

            _quitButton.Font = normalFont;
            _quitButton.InputProvider = ScreenManager.InputProvider;

            base.LoadContent();
        }

        void _quitButton_Selected(object sender, EventArgs e)
        {
            if (_quitMessageBox.IsVisible)
                return;
            //TODO: Add confirmation

            _quitMessageBox.Show("Do you really want to quit this game?", MessageBoxButtons.YesNo);

            //ScreenManager.Game.Exit();
        }

        void _levelsButton_Selected(object sender, EventArgs e)
        {
            if (_quitMessageBox.IsVisible)
                return;

            GameScreen screen = new LevelsScreen(spriteBatch);

            ScreenManager.Add(screen);
            ScreenManager.Activate(screen);
        }

        void _startButton_Selected(object sender, EventArgs e)
        {
            if (_quitMessageBox.IsVisible)
                return;

            LevelDefinition levelInfo = AtomixGame.State.GetCurrentLevel();

            Level currentLevel = LevelFactory.Load(string.Format("Content/Levels/{0}.atx", levelInfo.AssetName));

            LevelScreen gameScreen = new LevelScreen(currentLevel, spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        public override void UnloadContent() { }
    }
}
