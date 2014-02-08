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

        public StartScreen(AtomixGame game, SpriteBatch spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            // Recognize a single click of the left mouse button
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // React to the click
                Level currentLevel = game.Content.Load<AtomixData.Level>("Levels/Level6");
                LevelScreen gameScreen = new LevelScreen(game, currentLevel, spriteBatch);

                game.ChangeScreen(gameScreen);
            }
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string name = "Kinectomix";
            Vector2 size = splashFont.MeasureString(name);

            spriteBatch.DrawString(splashFont, name, new Vector2(game.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X/2, game.GraphicsDevice.Viewport.Bounds.Height / 2 - 100), Color.Black);

            name = "click screen to start the game";
            size = normalFont.MeasureString(name);
            
            spriteBatch.DrawString(normalFont, name, new Vector2(game.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X/2, game.GraphicsDevice.Viewport.Bounds.Height / 2 + 40), Color.Black);

            spriteBatch.End();
        }

        public void LoadContent()
        {
            splashFont = game.Content.Load<SpriteFont>("Fonts/Splash");
            normalFont = game.Content.Load<SpriteFont>("Fonts/Normal");
        }

        public void UnloadContent() { }
    }
}
