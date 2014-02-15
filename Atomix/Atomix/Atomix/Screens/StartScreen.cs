using AtomixData;
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
        IInputProvider input;

        public StartScreen(SpriteBatch spriteBatch, IInputProvider input)
        {
            this.spriteBatch = spriteBatch;
            this.input = input;
        }

        public override void Update(GameTime gameTime)
        {
            _startButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _startButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
            _startButton.Update(gameTime, input);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string name = "Kinectomix";
            Vector2 size = splashFont.MeasureString(name);

            spriteBatch.DrawString(splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 100), Color.Black);

            _startButton.Draw(gameTime);

            spriteBatch.End();
        }

        Button _startButton;

        public override void LoadContent()
        {
            splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _startButton = new Button(spriteBatch, "play game");
            _startButton.Font = normalFont;
            _startButton.LoadContent(ScreenManager.Content);
            _startButton.Selected += _startButton_Selected;
        }

        void _startButton_Selected(object sender, EventArgs e)
        {
            // React to the click
            LevelDefinition levelInfo = AtomixGame.State.GetCurrentLevel();
            Level currentLevel = ScreenManager.Content.Load<AtomixData.Level>("Levels/" + levelInfo.AssetName);
            LevelScreen gameScreen = new LevelScreen(currentLevel, spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        public override void UnloadContent() { }
    }
}
