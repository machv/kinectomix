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
    public class StartScreen : IGameScreen
    {
        AtomixGame game;
        SpriteFont splashFont;
        SpriteFont normalFont;
        SpriteBatch spriteBatch;
        IInputProvider input;

        public StartScreen(AtomixGame game, SpriteBatch spriteBatch, IInputProvider input)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.input = input;
        }

        public void Update(GameTime gameTime)
        {
            _startButton.Position = new Vector2(game.GraphicsDevice.Viewport.Bounds.Width / 2 - _startButton.Width / 2, game.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
            _startButton.Update(gameTime, input);
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string name = "Kinectomix";
            Vector2 size = splashFont.MeasureString(name);

            spriteBatch.DrawString(splashFont, name, new Vector2(game.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X/2, game.GraphicsDevice.Viewport.Bounds.Height / 2 - 100), Color.Black);

            _startButton.Draw(gameTime);

            spriteBatch.End();
        }

        Button _startButton;

        public void LoadContent()
        {
            splashFont = game.Content.Load<SpriteFont>("Fonts/Splash");
            normalFont = game.Content.Load<SpriteFont>("Fonts/Normal");

            _startButton = new Button(spriteBatch, "play game");
            _startButton.Font = normalFont;
            _startButton.LoadContent(game);
            _startButton.Selected += _startButton_Selected;
        }

        void _startButton_Selected(object sender, EventArgs e)
        {
            // React to the click
            Level currentLevel = game.Content.Load<AtomixData.Level>("Levels/Level1");
            LevelScreen gameScreen = new LevelScreen(game, currentLevel, spriteBatch);

            game.ChangeScreen(gameScreen);
        }

        public void UnloadContent() { }
    }
}
