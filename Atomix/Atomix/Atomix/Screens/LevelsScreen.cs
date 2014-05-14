using Kinectomix.Logic;
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
        SpriteBatch _spriteBatch;
        List<Button> _buttons = new List<Button>();
        SpriteFont _normalFont;

        public LevelsScreen(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 position = new Vector2(30, 60);

            foreach (Button b in _buttons)
            {
                b.Position = position;

                position.Y += b.Height + 10;
                position.X += 10;

                b.Update(gameTime, ScreenManager.InputProvider);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            foreach (Button b in _buttons)
                b.Draw(gameTime);

            _spriteBatch.End();
        }

        public override void LoadContent()
        {
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            foreach (var level in AtomixGame.State.Levels)
            {
                Button button = new Button(_spriteBatch);
                button.Font = _normalFont;
                button.LoadContent(ScreenManager.Content);
                button.Selected += button_Selected;
                button.Tag = level;
                button.Content = level.Name;

                _buttons.Add(button);
            }
        }

        void button_Selected(object sender, EventArgs e)
        {
            Button button = sender as Button;
            LevelDefinition level = button.Tag as LevelDefinition;

            Level currentLevel = ScreenManager.Content.Load<Kinectomix.Logic.Level>("Levels/" + level.AssetName);
            LevelScreen gameScreen = new LevelScreen(currentLevel, _spriteBatch);

            AtomixGame.State.SetLevelToCurrent(level);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        public override void UnloadContent() { }
    }
}
