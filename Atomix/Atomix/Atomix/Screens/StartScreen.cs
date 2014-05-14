using KinectomixLogic;
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

        public StartScreen(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            _startButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _startButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 80);
            _startButton.Update(gameTime, ScreenManager.InputProvider);

            _levelsButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _startButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 20);
            _levelsButton.Update(gameTime, ScreenManager.InputProvider);

            _quitButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _quitButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 50 + _quitButton.Height);
            _quitButton.Update(gameTime, ScreenManager.InputProvider);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string name = "Kinectomix";
            Vector2 size = splashFont.MeasureString(name);

            spriteBatch.DrawString(splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 220), Color.Black);

            _startButton.Draw(gameTime);
            _levelsButton.Draw(gameTime);
            _quitButton.Draw(gameTime);

            spriteBatch.End();
        }

        public override void LoadContent()
        {
            splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _startButton = new Button(spriteBatch, "play game");
            _startButton.Font = normalFont;
            _startButton.LoadContent(ScreenManager.Content);
            _startButton.Selected += _startButton_Selected;

            _levelsButton = new Button(spriteBatch, "levels");
            _levelsButton.Font = normalFont;
            _levelsButton.LoadContent(ScreenManager.Content);
            _levelsButton.Selected += _levelsButton_Selected;

            _quitButton = new Button(spriteBatch, "quit game");
            _quitButton.Font = normalFont;
            _quitButton.LoadContent(ScreenManager.Content);
            _quitButton.Selected += _quitButton_Selected;
        }

        void _quitButton_Selected(object sender, EventArgs e)
        {
            //TODO: Add confirmation

            ScreenManager.Game.Exit();
        }

        void _levelsButton_Selected(object sender, EventArgs e)
        {
            GameScreen screen = new LevelsScreen(spriteBatch);

            ScreenManager.Add(screen);
            ScreenManager.Activate(screen);
        }

        void _startButton_Selected(object sender, EventArgs e)
        {
            LevelDefinition levelInfo = AtomixGame.State.GetCurrentLevel();

            Level currentLevel = LevelLoader.Load(string.Format("Content/Levels/{0}.atx", levelInfo.AssetName));

            //Level currentLevel = ScreenManager.Content.Load<AtomixData.Level>("Levels/" + levelInfo.AssetName);
            LevelScreen gameScreen = new LevelScreen(currentLevel, spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        public override void UnloadContent() { }
    }
}
