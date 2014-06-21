using Kinectomix.Logic;
using Kinectomix.Xna.Components;
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

                b.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            foreach (Button b in _buttons)
                b.Draw(gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadContent()
        {
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            foreach (Level level in AtomixGame.State.Levels)
            {
                Button button = new Button(ScreenManager.Game);
                button.Font = _normalFont;
                button.Selected += button_Selected;
                button.Tag = level;
                button.Content = level.Name;

                _buttons.Add(button);
            }

            base.LoadContent();
        }

        void button_Selected(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Level level = button.Tag as Level;

            //Level currentLevel = LevelFactory.Load(string.Format("Content/Levels/{0}.atx", level.AssetName));
            LevelScreen gameScreen = new LevelScreen(level, _spriteBatch);

            AtomixGame.State.SetLevelToCurrent(level);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }
    }
}
